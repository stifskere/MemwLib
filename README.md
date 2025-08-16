# MemwLib

> [!warning]
> **This repository is now archived**, I cease the mainteinance of this repository as it hasn't been updated since 2024,
> I now moved on to rust.
>
> This repository is a merge between a utils library and a complete HTTP library, it doesn't support basic stuff such as
> encoding, I used it to learn how production frameworks worked, even tho it never became one. HTTP is trivial anyways.

MemwLib is meant for these who want a fast and reliable HTTP server environment. Taking the best from all the HTTP libraries out there, MemwLib offers you utilities like Json parsing, Environment variable parsing... And more.

## Getting Started

The documentation for this library is being built in my documentation site (wip), meanwhile you can rely on the XML documentation this library provides.

Since this library is very big it's still work in progress and it will accept any contribution.

### Prerequisites

All you need to install this library you need a .NET `>= 7.0` solution.

### Installation

You can get this library from NuGet under `MemwLib`, so the installation steps are the following

```bash
dotnet add package MemwLib
```

## Repository structure

This repository is based on a simple structure, when you make your PR you want to merge it to the dev branch,
that branch is the one that's active on development and testing, when your PR is merged it will be tested and
merged to main. When anything gets merged to main it automatically deploys to NuGet trough a github action and
a release is created.

### Branches

* Main: Deployment branch, everything in here will be released.
* Dev: Active development and testing branch.
