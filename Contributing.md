# Contributing to OpenSilver

Thanks for taking the time to contribute to OpenSilver! It is people like you who make OpenSilver a powerful Silverlight replacement, capable of bringing back the power of C#, XAML, and .NET to client-side Web development.

OpenSilver is an open source project and we love to receive contributions from our community — you! There are many ways to contribute, from writing tutorials or blog posts, improving the documentation, submitting bug reports and feature requests or writing code which can be incorporated into OpenSilver itself.

Any contribution is welcome, being it a big one or a small one, including fixing spelling/grammar errors, correcting typos, cleaning up the code, etc.

We usually merge Pull Requests within 48 hours.

## How to make a pull request

1. Fork the repository
2. Optionally, create an issue for any major change or enhancement that you wish to make, so as to get feedback from the community (not required though)
3. Do your changes on the "develop" branch (or another branch derived from "develop") of your fork. Please refer to the [Readme](README.md) file for instructions on how to build and run
4. Create unit tests if needed (see below)
5. Verify that your changes do not cause regressions. To do so, please run the unit tests (see below), and verify that both CSHTML5 and OpenSilver work properly. If you are making a change to OpenSilver and you do not have much time to test CSHTML5, please verify at least that the CSHTML5.sln solution still compiles properly.
6. Make sure that your code is up to date by rebasing your branch on the upstream "develop" branch
7. Submit the PR to the "develop" branch

## How to create or run unit tests

There are currently 2 types of tests:

#### 1. Tests that do not require a GUI

They are located in the **`Runtime.OpenSilver.Tests`** project which is contained in the main `OpenSilver.sln` solution. It is a unit tests project which uses the [MSTest](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-mstest) testing framework.

#### 2. Tests that require a GUI

They are located in the **`TestApplication`** project which is contained in the main `OpenSilver.sln` solution. It is a project of type OpenSilver that is intended to be run either in the browser or in the Simulator. The same project also exists in Silverlight so that you can compare the result of the OpenSilver version and the Silverlight version of the GUI.

*Note: the test projects are currently work-in-progress: we are reorganizing the TestApplication so that the features are split into categories with a menu to navigate.*

## How to contribute to the documention

Please refer to the instructions [here](https://github.com/OpenSilver/OpenSilver.Documentation).

## How to contribute to the Showcase app

We welcome contributons to the Showcase app! Its source code is located [here](https://github.com/cshtml5/CSHTML5.Samples.Showcase).

# License

Please read [LICENSE.txt](LICENSE.txt) and [THIRD-PARTY-NOTICES.txt](THIRD-PARTY-NOTICES.txt) for license information.

By contributing to OpenSilver, you accept and agree that your present and future contributions submitted to OpenSilver be bound by the terms and conditions of the OpenSilver license (available [here](LICENSE.txt)).

If you use code from other open-source software, please specify it in your Pull Requests, as well as in the header of the submitted files, and be sure to add the corresponding notice to [THIRD-PARTY-NOTICES.txt](THIRD-PARTY-NOTICES.txt).


# Code of Conduct

Please find the Code of Conduct [here](https://www.contributor-covenant.org/version/1/4/code-of-conduct/).


# Contacting the OpenSilver core team

Contact information can be found [here](https://opensilver.net/contact.aspx)


