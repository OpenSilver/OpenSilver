# Contributing to OpenSilver

Thanks for taking the time to contribute to OpenSilver! It is people like you who make OpenSilver a powerful framework for bringing the power of C#, XAML, and .NET to cross-platform development.

OpenSilver is an open source project and we love to receive contributions from our community! There are many ways to contribute, from writing tutorials or blog posts, improving the documentation, submitting bug reports and feature requests, or writing code which can be incorporated into OpenSilver itself.

Any contribution is welcome, be it a big one or a small one, including fixing spelling/grammar errors, correcting typos, cleaning up the code, etc.

## How to Build and Run

For instructions on how to build OpenSilver from source, including repository structure and branch information, see [BUILDING.md](BUILDING.md).

## How to Make a Pull Request

1. Fork the repository
2. Optionally, create an issue for any major change or enhancement that you wish to make, so as to get feedback from the community (not required though)
3. Do your changes on the `develop` branch (or another branch derived from `develop`) of your fork. See [BUILDING.md](BUILDING.md) for instructions on how to build and run.
4. Create unit tests if needed (see below)
5. Verify that your changes do not cause regressions by running the unit tests (see below)
6. Make sure that your code is up to date by rebasing your branch on the upstream `develop` branch
7. Submit the PR to the `develop` branch

## How to Create or Run Unit Tests

There are currently 2 types of tests:

#### 1. Tests that do not require a GUI

They are located in the **`Runtime.OpenSilver.Tests`** project which is contained in the main `OpenSilver.sln` solution. It is a unit tests project which uses the [MSTest](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-mstest) testing framework.

#### 2. Tests that require a GUI

They are located in the **`TestApplication`** project which is contained in the main `OpenSilver.sln` solution. It is a project of type OpenSilver that is intended to be run either in the browser or in the Simulator.

## How to Contribute to the Documentation

Please refer to the instructions [here](https://github.com/OpenSilver/OpenSilver.Documentation).

## How to Contribute to the Showcase App

We welcome contributions to the Showcase app! Its source code is located [here](https://github.com/OpenSilver/OpenSilver.Samples.Showcase).

## License

Please read [LICENSE.txt](LICENSE.txt) and [THIRD-PARTY-NOTICES.txt](THIRD-PARTY-NOTICES.txt) for license information.

By contributing to OpenSilver, you accept and agree that your present and future contributions submitted to OpenSilver be bound by the terms and conditions of the OpenSilver license (available [here](LICENSE.txt)).

If you use code from other open-source software, please specify it in your Pull Requests, as well as in the header of the submitted files, and be sure to add the corresponding notice to [THIRD-PARTY-NOTICES.txt](THIRD-PARTY-NOTICES.txt).

## Code of Conduct

Please find the Code of Conduct [here](https://www.contributor-covenant.org/version/1/4/code-of-conduct/).

## Contacting the OpenSilver Core Team

Contact information can be found [here](https://opensilver.net/contact.aspx).
