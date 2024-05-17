# PixelPrime

## Maintainers

- [Maximilian Huber](https://github.com/maxiboy441)
- [Rui Felipe Pedrosso Ramos](https://github.com/feliperamoss)
- [Amir Hassan](https://github.com/AmirHassanMojahed)
- [Marius Wenzel](https://github.com/Mariuswenzelits)
- [Mehrulloh Boboev](https://github.com/mehrb98)


## Workflow

### Conventional Commits

This project follows the [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/) specification for commit messages. This convention provides a clear and consistent way to write commit messages, making it easier to understand the changes made to the codebase and facilitate automated tooling for generating changelogs and versioning.

The commit types and their meanings are as follows:

- **feat**: A new feature
- **fix**: A bug fix
- **docs**: Documentation changes
- **style**: Changes that do not affect the meaning of the code (formatting, missing semi-colons, etc.)
- **refactor**: A code change that neither fixes a bug nor adds a feature
- **perf**: A code change that improves performance
- **test**: Adding missing tests or correcting existing tests
- **build**: Changes that affect the build system or external dependencies
- **ci**: Changes to our CI configuration files and scripts
- **chore**: Other changes that don't modify source or test files
- **revert**: Reverts a previous commit

So commitmessages should look like: 
{type}({ticket/issues}):{description}

### Git Flow

The project uses the [Git Flow](https://nvie.com/posts/a-successful-git-branching-model/) branching model, which consists of the following main branches:

- `main`: This is the main branch where the source code of the latest stable release lives.
- `develop`: This is the integration branch for features and bug fixes. All development work happens on feature branches branched off from `develop`.

#### Feature Branches

For every new feature or bug fix, a new branch should be created from `develop`. The naming convention for feature branches is `feature/your-feature-name` or `fix/your-bug-fix-name`.

Once the work on the feature or bug fix is complete, create a pull request from the feature branch to `develop`. The pull request will be reviewed, and after approval, it will be merged into `develop`.

#### Release Branches

When it's time to create a new release, a release branch is created from `develop`. The naming convention for release branches is `release/x.y.z`, where `x.y.z` represents the version number of the upcoming release.

On the release branch, any necessary release preparations can be made, such as updating documentation, performing final testing, and merging any last-minute bug fixes. Once the release is ready, it is merged into both `main` and `develop` branches, and a tag is created on `main` for the new release version.

#### Hotfix Branches

If a critical bug is discovered in a released version, a hotfix branch is created from the corresponding release tag on `main`. The naming convention for hotfix branches is `hotfix/your-hotfix-name`.

Once the bug is fixed, the hotfix branch is merged into both `main` and `develop`, and a new tag is created on `main` for the patched release version.
