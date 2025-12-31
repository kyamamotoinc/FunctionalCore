# FunctionalCore

**FunctionalCore** is a lightweight C# library that provides core functional programming primitives.

## Overview

This library provides the following types:

- `Result<E, T>` — Represents a computation that can succeed (`T`) or fail (`E`).
- `Option<T>` — Represents an optional value (`Some` or `None`), similar to `Maybe`.
- `Unit` — Represents a value-less success, useful for operations that only indicate success or failure.

These types help write safer and more expressive code by embracing functional programming patterns in C#.

## Installation

Currently, `FunctionalCore` is not available as a NuGet package. Clone the repository and add the source files to your project:
