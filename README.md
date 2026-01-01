# FunctionalCore

**FunctionalCore** is a small C# library providing functional data types inspired by functional programming languages, focusing on error handling and optional values. It currently includes:

`Result<E, T>` – represents computations that may succeed (`T`) or fail (`E`).

`Option<T>` – represents values that may or may not exist (`Some` or `None`).

`Unit` – represents a value-less success, useful for operations that only indicate success or failure.

## Overview

`Result<E, T>` was inspired by Haskell's `Either<L, R>`.
While Either is fully general-purpose, Result limits its role to representing success or failure, providing a clear and intuitive API for handling errors in C#.
This design improves code readability, reduces misuse, and aligns with common functional patterns for computations that may fail.

`Option<T>` is inspired by the `Maybe` type in Haskell. It represents a value that may or may not exist, helping to eliminate null references and make optional data explicit in code.

## Features

`Functional combinators`: Map, Bind, Select, SelectMany for LINQ support

`Tap helpers`: Tap, TapError, TapAll for side-effects

`Value extraction`: Match, ValueOrDefault, ValueOrThrow

`Applicative-style composition`: Combine for combining multiple results or options

## Philosophy

This library focuses on clarity and safety, favoring explicit handling of success, failure, and optional values over exceptions and nulls.
It is intentionally minimalistic, making it easy to adopt in any C# project without heavy dependencies.

## Installation

Currently, `FunctionalCore` is not available as a NuGet package. Clone the repository and add the source files to your project:
