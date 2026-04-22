#!/usr/bin/env bash
#
# Installs this repo's Claude Code skills into ~/.claude/skills/ so that
# Claude Code picks them up on the next session. Existing skill directories
# of the same name are overwritten.
#
# Usage: ./.claude/install-skills.sh

set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
repo_skills_dir="$repo_root/.claude/skills"
user_skills_dir="$HOME/.claude/skills"

if [[ ! -d "$repo_skills_dir" ]]; then
    echo "No skills found at $repo_skills_dir" >&2
    exit 1
fi

mkdir -p "$user_skills_dir"

installed=0
for skill_dir in "$repo_skills_dir"/*/; do
    [[ -d "$skill_dir" ]] || continue
    skill_name="$(basename "$skill_dir")"
    target="$user_skills_dir/$skill_name"

    rm -rf "$target"
    cp -R "$skill_dir" "$target"
    echo "Installed skill: $skill_name -> $target"
    installed=$((installed + 1))
done

if (( installed == 0 )); then
    echo "No skill directories found under $repo_skills_dir"
else
    echo ""
    echo "Installed $installed skill(s). Restart your Claude Code session to pick them up."
fi
