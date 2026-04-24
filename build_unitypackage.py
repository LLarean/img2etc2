#!/usr/bin/env python3
"""Builds a .unitypackage from files with corresponding .meta files."""

import os
import re
import sys
import tarfile
import tempfile

EXCLUDE_DIRS = {'.git', '.github', '.idea'}
EXCLUDE_FILES = {'build_unitypackage.py'}

REPO_ROOT = os.path.dirname(os.path.abspath(__file__))


def read_guid(meta_path):
    with open(meta_path, 'r', encoding='utf-8') as f:
        for line in f:
            m = re.match(r'^guid:\s*([a-f0-9]+)', line)
            if m:
                return m.group(1)
    return None


def collect_assets():
    assets = []
    for dirpath, dirnames, filenames in os.walk(REPO_ROOT):
        dirnames[:] = [d for d in dirnames if d not in EXCLUDE_DIRS]
        for fname in filenames:
            if fname in EXCLUDE_FILES:
                continue
            if fname.endswith('.meta'):
                continue
            full_path = os.path.join(dirpath, fname)
            meta_path = full_path + '.meta'
            if not os.path.exists(meta_path):
                print(f'WARNING: no .meta for {full_path}, skipping', file=sys.stderr)
                continue
            guid = read_guid(meta_path)
            if not guid:
                print(f'WARNING: no guid in {meta_path}, skipping', file=sys.stderr)
                continue
            rel_path = os.path.relpath(full_path, REPO_ROOT).replace('\\', '/')
            assets.append((guid, full_path, meta_path, rel_path))
    return assets


def build(output_path):
    assets = collect_assets()
    with tempfile.TemporaryDirectory() as tmpdir:
        for guid, asset_path, meta_path, rel_path in assets:
            guid_dir = os.path.join(tmpdir, guid)
            os.makedirs(guid_dir)
            import shutil
            shutil.copy2(asset_path, os.path.join(guid_dir, 'asset'))
            shutil.copy2(meta_path, os.path.join(guid_dir, 'asset.meta'))
            with open(os.path.join(guid_dir, 'pathname'), 'w', encoding='utf-8') as f:
                f.write(rel_path)
        with tarfile.open(output_path, 'w:gz') as tar:
            tar.add(tmpdir, arcname='')
    print(f'Built: {output_path} ({len(assets)} assets)')


if __name__ == '__main__':
    out = sys.argv[1] if len(sys.argv) > 1 else 'IMG2ETC2.unitypackage'
    build(out)
