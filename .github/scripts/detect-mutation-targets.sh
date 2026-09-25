#!/usr/bin/env bash
# Maps .cs files changed since <base-ref> to Stryker `--mutate` globs, one per line in <output-file>.
# Works with any folder layout: a file belongs to the nearest folder containing a *.csproj.
#  - file in a source project -> that file
#  - file in a test project   -> file(s) declaring the type under test, found by name
#                                (file name without Tests/Test suffix, then parent folder names)
#                                in the non-test projects referenced by the test project;
#                                no match -> every .cs file of those projects
#  - anything else            -> ignored (non-.cs files, .cs files outside any project)
# Test project = folder or .csproj name with a Test/Tests segment (Demo.Domain.Tests, Payments.Core.UnitTests,
# Payments.Tests.Shared), or a .csproj with <IsTestProject>true</IsTestProject> or a Microsoft.NET.Test.Sdk reference.
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

find_project_dir() { # $1 = repo-relative file path -> nearest ancestor directory containing a *.csproj ("." = repo root)
  local dir
  dir="$(dirname "$1")"
  while true; do
    if compgen -G "$dir/*.csproj" > /dev/null; then
      echo "$dir"
      return 0
    fi
    if [[ "$dir" == "." ]]; then
      return 1
    fi
    dir="$(dirname "$dir")"
  done
}

is_test_project() { # $1 = project dir
  local csproj name_regex='(^|\.)[A-Za-z0-9]*Tests?(\.|$)'
  if [[ "$(basename "$(realpath -m "$1")")" =~ $name_regex ]]; then
    return 0
  fi
  for csproj in "$1"/*.csproj; do
    [[ -f "$csproj" ]] || continue
    if [[ "$(basename "$csproj" .csproj)" =~ $name_regex ]] \
      || grep -qiE '<IsTestProject>[[:space:]]*true[[:space:]]*</IsTestProject>|Include="Microsoft\.NET\.Test\.Sdk"' "$csproj"; then
      return 0
    fi
  done
  return 1
}

project_glob() { # $1 = project dir -> glob matching all its .cs files
  if [[ "$1" == "." ]]; then
    echo "**/*.cs"
  else
    echo "**/$1/**/*.cs"
  fi
}

referenced_projects() { # $1 = project dir -> repo-relative dirs of the projects in its <ProjectReference> items
  local csproj ref
  for csproj in "$1"/*.csproj; do
    [[ -f "$csproj" ]] || continue
    grep -oE '<ProjectReference[^>]*Include="[^"]+"' "$csproj" \
      | sed -E 's/.*Include="([^"]+)"/\1/' \
      | tr '\\' '/' \
      | while IFS= read -r ref; do
          dirname "$(realpath -m --relative-to=. "$1/$ref")"
        done
  done | grep -v '^\.\./' | sort -u || true
}

find_type_declarations() { # $1 = type name, rest = dirs to search
  local name="$1"
  shift
  [[ "$name" =~ ^[A-Za-z_][A-Za-z0-9_]*$ ]] || return 0
  grep -rlE --include='*.cs' --exclude-dir=bin --exclude-dir=obj \
    "\b(class|record|struct|interface)[[:space:]]+${name}\b" "$@" | sed 's|^\./||' | sort || true
}

map_test_file() { # $1 = changed test file, $2 = its test project dir
  local test_file="$1" project_dir="$2" stem name rel_dir dir match
  local -a referenced src_dirs candidates folders matches

  mapfile -t referenced < <(referenced_projects "$project_dir")
  src_dirs=()
  for dir in "${referenced[@]}"; do
    if ! is_test_project "$dir"; then
      src_dirs+=("$dir")
    fi
  done
  if [[ ${#src_dirs[@]} -eq 0 ]]; then
    echo "  (skipped $test_file: $project_dir references no source project)" >&2
    return 0
  fi

  stem="$(basename "$test_file" .cs)"
  name="${stem%Tests}"
  if [[ "$name" == "$stem" ]]; then
    name="${stem%Test}"
  fi
  candidates=("$name")

  rel_dir="$(dirname "$test_file")"
  if [[ "$project_dir" == "." ]]; then
    if [[ "$rel_dir" == "." ]]; then
      rel_dir=""
    fi
  else
    rel_dir="${rel_dir#"$project_dir"}"
    rel_dir="${rel_dir#/}"
  fi
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

  for dir in "${src_dirs[@]}"; do
    add_target "$(project_glob "$dir")" "test \`$test_file\` -> no type match, whole project \`$dir\`"
  done
}

mapfile -t changed_files < <(git diff --name-only --diff-filter=AMR "$base_ref...HEAD" -- '*.cs')

for file in "${changed_files[@]}"; do
  if ! project_dir="$(find_project_dir "$file")"; then
    continue
  fi
  if is_test_project "$project_dir"; then
    map_test_file "$file" "$project_dir"
  else
    add_target "**/$file" "changed \`$file\`"
  fi
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
