#!/usr/bin/env bash
# Maps files changed since <base-ref> to Stryker `--mutate` globs, one per line in <output-file>.
#  - src/**/*.cs                -> that file
#  - tests/**/*.cs              -> file(s) declaring the class under test, found by name
#                                  (test file name without Tests/Test suffix, then parent folder names);
#                                  no match -> every .cs file of the src project(s) referenced by the test project
#  - anything else              -> ignored
# Prints a markdown list with the reason for each target.
set -euo pipefail

base_ref="${1:?usage: detect-mutation-targets.sh <base-ref> <output-file>}"
output_file="${2:?usage: detect-mutation-targets.sh <base-ref> <output-file>}"

repo_root="$(git rev-parse --show-toplevel)"
cd "$repo_root"

declare -a target_order=()
declare -A target_reasons=()

add_target() { # $1 = glob, $2 = reason
  if [[ -z "${target_reasons[$1]+x}" ]]; then
    target_order+=("$1")
    target_reasons["$1"]="$2"
  else
    target_reasons["$1"]+="; $2"
  fi
}

find_project_dir() { # $1 = file path -> nearest ancestor directory containing a *.csproj
  local dir
  dir="$(dirname "$1")"
  while [[ "$dir" != "." && "$dir" != "/" ]]; do
    if compgen -G "$dir/*.csproj" > /dev/null; then
      echo "$dir"
      return 0
    fi
    dir="$(dirname "$dir")"
  done
  return 1
}

referenced_src_projects() { # $1 = test project dir -> src project dirs from its <ProjectReference> items
  local csproj ref
  for csproj in "$1"/*.csproj; do
    grep -oE '<ProjectReference[^>]*Include="[^"]+"' "$csproj" \
      | sed -E 's/.*Include="([^"]+)"/\1/' \
      | tr '\\' '/' \
      | while IFS= read -r ref; do
          dirname "$(realpath -m --relative-to=. "$1/$ref")"
        done
  done | grep -E '^src/' | sort -u || true
}

find_type_declarations() { # $1 = type name, rest = dirs to search
  local name="$1"
  shift
  [[ "$name" =~ ^[A-Za-z_][A-Za-z0-9_]*$ ]] || return 0
  grep -rlE --include='*.cs' --exclude-dir=bin --exclude-dir=obj \
    "\b(class|record|struct|interface)[[:space:]]+${name}\b" "$@" | sort || true
}

map_test_file() { # $1 = changed test file
  local test_file="$1" project_dir stem name rel_dir folder matches
  local -a src_dirs candidates folders

  if ! project_dir="$(find_project_dir "$test_file")"; then
    echo "  (skipped $test_file: no test project found)" >&2
    return 0
  fi

  mapfile -t src_dirs < <(referenced_src_projects "$project_dir")
  if [[ ${#src_dirs[@]} -eq 0 ]]; then
    echo "  (skipped $test_file: $project_dir references no src project)" >&2
    return 0
  fi

  stem="$(basename "$test_file" .cs)"
  name="${stem%Tests}"
  [[ "$name" == "$stem" ]] && name="${stem%Test}"
  candidates=("$name")

  rel_dir="$(dirname "$test_file")"
  rel_dir="${rel_dir#"$project_dir"}"
  rel_dir="${rel_dir#/}"
  if [[ -n "$rel_dir" ]]; then
    IFS='/' read -r -a folders <<< "$rel_dir"
    for (( i=${#folders[@]}-1; i>=0; i-- )); do
      candidates+=("${folders[i]}")
    done
  fi

  for name in "${candidates[@]}"; do
    [[ -n "$name" ]] || continue
    mapfile -t matches < <(find_type_declarations "$name" "${src_dirs[@]}")
    if [[ ${#matches[@]} -gt 0 ]]; then
      for match in "${matches[@]}"; do
        add_target "**/$match" "test \`$test_file\` -> type \`$name\`"
      done
      return 0
    fi
  done

  for src_dir in "${src_dirs[@]}"; do
    add_target "**/$src_dir/**/*.cs" "test \`$test_file\` -> no type match, whole project \`$src_dir\`"
  done
}

mapfile -t changed_files < <(git diff --name-only --diff-filter=AMR "$base_ref...HEAD")

for file in "${changed_files[@]}"; do
  case "$file" in
    src/*.cs)   add_target "**/$file" "changed \`$file\`" ;;
    tests/*.cs) map_test_file "$file" ;;
  esac
done

: > "$output_file"
if [[ ${#target_order[@]} -eq 0 ]]; then
  echo "No changed source or test code since \`$base_ref\`."
  exit 0
fi

echo "Mutation targets since \`$base_ref\`:"
echo
for glob in "${target_order[@]}"; do
  echo "$glob" >> "$output_file"
  echo "- \`$glob\` (${target_reasons[$glob]})"
done
