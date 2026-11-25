# Agent notes

Instructions for anyone (human or coding agent) changing this repository.

## Layout

UPM package `com.llarean.img2etc2`: a Unity editor extension that resizes
textures to ETC2/Crunch-compatible dimensions (multiples of 4). All code
is editor-only, in `Editor/` (namespace `LLarean.IMG2ETC2`).
`Documentation~/` holds working notes (Unity ignores `~` folders, no
`.meta` needed there). `build_unitypackage.py` and
`check_unitypackage.py` build and validate the `.unitypackage`
distribution channel.

## Commands

| Task | Command |
| --- | --- |
| Build .unitypackage | `python3 build_unitypackage.py IMG2ETC2.unitypackage` |
| Validate archive | `python3 check_unitypackage.py IMG2ETC2.unitypackage` |
| Lint Markdown | `npx --yes markdownlint-cli2 "**/*.md"` |

CI runs the same checks on every push and pull request.

## Conventions

- The improvement plan lives in
  [Documentation~/FIX_PLAN.md](Documentation~/FIX_PLAN.md) — read it
  before non-trivial work and keep it current as items land.
- Code in `Editor/` depends on UnityEditor: there is no plain-.NET
  build, and CI does not compile it. Verify code changes in the Unity
  Editor (batch mode is fine) before committing; never push untested
  code changes.
- Every `.cs` file needs a Unity `.meta` next to it (fresh GUID).
- New runtime behavior gets an EditMode test once the test track lands
  (see «Порядок работ» in FIX_PLAN).
- Release: bump `version` in `package.json`, tag `vX.Y.Z` matching it
  (CI enforces the match), push the tag — the release workflow builds,
  validates and publishes the `.unitypackage`.
- Small commits, green CI at every step.

## Traps

- In Git Bash, `git show rev:path` is mangled by MSYS path conversion —
  prefix the command with `MSYS_NO_PATHCONV=1`.
- `.unitypackage` pathname entries must start with `Assets/IMG2ETC2/`;
  the validator fails the build otherwise.
- The historical tag `v1.7.0` points at a commit whose package.json says
  1.6.0; it stays for reproducibility. New tags must match the manifest.
