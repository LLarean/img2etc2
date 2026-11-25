#!/usr/bin/env python3
"""Validates a .unitypackage built by build_unitypackage.py.

Checks the archive against the repository: required files, Assets/
prefix, GUID correspondence with the repo .meta files, no duplicate
paths or GUIDs. Exits non-zero with a message on the first problem.
"""

import os
import sys
import tarfile

from build_unitypackage import ASSET_PREFIX, collect_assets


def fail(message):
    print(f'ERROR: {message}', file=sys.stderr)
    sys.exit(1)


def read_archive(output_path):
    entries = {}
    with tarfile.open(output_path, 'r:gz') as tar:
        for member in tar.getmembers():
            if not member.isfile():
                continue
            guid, _, kind = member.name.partition('/')
            if kind not in ('asset', 'asset.meta', 'pathname'):
                continue
            data = tar.extractfile(member).read()
            entries.setdefault(guid, {})[kind] = data
    return entries


def main():
    if len(sys.argv) < 2:
        fail('usage: check_unitypackage.py <archive.unitypackage>')
    entries = read_archive(sys.argv[1])
    if not entries:
        fail('archive contains no asset entries')

    pathnames = {}
    for guid, parts in entries.items():
        if 'pathname' not in parts or 'asset.meta' not in parts:
            fail(f'{guid}: entry must contain pathname and asset.meta')
        pathname = parts['pathname'].decode('utf-8').splitlines()[0].strip()
        if not pathname.startswith(ASSET_PREFIX + '/'):
            fail(f'{guid}: pathname {pathname!r} is outside {ASSET_PREFIX}/')
        if pathname in pathnames:
            fail(f'duplicate pathname {pathname!r} ({guid} and {pathnames[pathname]})')
        pathnames[pathname] = guid
        meta_guid = None
        for line in parts['asset.meta'].decode('utf-8').splitlines():
            if line.startswith('guid:'):
                meta_guid = line.split(':', 1)[1].strip()
        if meta_guid != guid:
            fail(f'{pathname}: entry dir {guid} does not match guid {meta_guid} in asset.meta')

    expected = {asset_rel: guid for guid, _, _, asset_rel in collect_assets()}
    for asset_rel, guid in expected.items():
        if asset_rel not in pathnames:
            fail(f'required asset missing from archive: {asset_rel}')
        if pathnames[asset_rel] != guid:
            fail(f'{asset_rel}: archive guid {pathnames[asset_rel]} != repo guid {guid}')

    print(f'OK: {len(entries)} entries, {len(expected)} required assets verified')


if __name__ == '__main__':
    main()
