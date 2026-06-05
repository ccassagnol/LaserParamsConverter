# Legacy WinForms source (read-only)

This folder preserves the original Windows-only WinForms implementation of
LaserParamsConverter v1.1.3 by David Christian, kept intact for reference while
the cross-platform port (under `/src`) is being built.

**Do not modify these files.** They are kept solely so we can:
- Compare on-disk byte output of the new parsers against the original.
- Look up any subtle behavior the original UI had (defaults, validation, etc.).

The active solution does not build anything from this folder. Once the cross-platform
port reaches feature parity, this folder may be removed; the upstream commit will
still be reachable via the `upstream/master` remote-tracking ref.

Upstream: <https://github.com/shark92651/LaserParamsConverter>
