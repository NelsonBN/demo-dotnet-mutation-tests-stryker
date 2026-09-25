using System;

namespace Demo.Domain;

public sealed class InvalidException(string message) : Exception(message);

public sealed class DuplicateException(string message) : Exception(message);

public sealed class NotFoundException(string message) : Exception(message);
