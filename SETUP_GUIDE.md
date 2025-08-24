# 🚀 GitHub Repository Setup Guide

This guide will walk you through setting up your GitHub repository for the Universal Enum Editor Unity package.

## 📋 Prerequisites

- GitHub account
- Git installed on your computer
- Unity 2019.4 or newer (for testing)

## 🎯 Step-by-Step Setup

### 1. Create GitHub Repository

1. Go to [GitHub](https://github.com) and sign in
2. Click the **+** button → **New repository**
3. Repository name: `UniversalEnumEditor`
4. Description: `A powerful Unity editor tool for managing enums with smart discovery, direct editing, and validation features.`
5. Make it **Public**
6. **Don't** initialize with README, .gitignore, or license (we'll add these manually)
7. Click **Create repository**

### 2. Clone Repository Locally

```bash
git clone https://github.com/iamhassan1/UniversalEnumEditor.git
cd UniversalEnumEditor
```

### 3. Copy Package Files

Copy the following files from your Unity project to the repository:

**Core Package Structure:**
```
UniversalEnumEditor/
├── package.json
├── Runtime/
│   └── README.md
├── Editor/
│   ├── UniversalEnumEditor.cs
│   ├── EnumPickerWindow.cs
│   └── EnumScriptDatabase.cs
└── Samples~/
    └── BasicUsage/
        ├── README.md
        ├── BasicEnums.cs
        ├── GameEnums.cs
        └── MultipleEnums.cs
```

**Repository Files:**
```
UniversalEnumEditor/
├── README.md
├── LICENSE
├── CHANGELOG.md
├── .gitignore
└── SETUP_GUIDE.md
```

### 4. Add Files to Git

```bash
# Add all files
git add .

# Initial commit
git commit -m "Initial release: Universal Enum Editor v1.0.0

- Smart script discovery and scanning
- Direct enum editing capabilities
- Real-time validation and error checking
- Search and filtering functionality
- Import/export features
- Professional Unity editor integration
- Comprehensive samples and documentation"

# Push to GitHub
git push origin main
```

### 5. Create GitHub Release

1. Go to your repository on GitHub
2. Click **Releases** → **Create a new release**
3. Tag version: `v1.0.0`
4. Release title: `Universal Enum Editor v1.0.0`
5. Description: Copy from CHANGELOG.md
6. Upload the Unity package file (if you have one)
7. Click **Publish release**

### 6. Set Up Repository Features

#### **Issues Template**
Create `.github/ISSUE_TEMPLATE/bug_report.md`:
```markdown
---
name: Bug report
about: Create a report to help us improve
title: ''
labels: bug
assignees: ''

---

**Describe the bug**
A clear and concise description of what the bug is.

**To Reproduce**
Steps to reproduce the behavior:
1. Go to '...'
2. Click on '....'
3. Scroll down to '....'
4. See error

**Expected behavior**
A clear and concise description of what you expected to happen.

**Unity Version**
- Unity: [e.g. 2022.3.0f1]

**Additional context**
Add any other context about the problem here.
```

#### **Pull Request Template**
Create `.github/pull_request_template.md`:
```markdown
## Description
Please include a summary of the change and which issue is fixed.

## Type of change
- [ ] Bug fix (non-breaking change which fixes an issue)
- [ ] New feature (non-breaking change which adds functionality)
- [ ] Breaking change (fix or feature that would cause existing functionality to not work as expected)

## Checklist
- [ ] My code follows the style guidelines of this project
- [ ] I have performed a self-review of my own code
- [ ] I have commented my code, particularly in hard-to-understand areas
- [ ] I have made corresponding changes to the documentation
- [ ] My changes generate no new warnings
```

### 7. Configure Repository Settings

1. Go to **Settings** → **Pages**
2. Source: **Deploy from a branch**
3. Branch: **main** → **Save**

### 8. Test Installation

1. Create a new Unity project
2. Open Package Manager: `Window > Package Manager`
3. Click **+** → **Add package from git URL**
4. Enter: `https://github.com/iamhassan1/UniversalEnumEditor.git`
5. Click **Add**
6. Verify the tool appears under `Tools > Universal Enum Editor`

## 🔧 Repository Structure

Your final repository should look like this:

```
UniversalEnumEditor/
├── .github/
│   ├── ISSUE_TEMPLATE/
│   │   └── bug_report.md
│   └── pull_request_template.md
├── Packages/
│   └── com.mhstools.universalenumeditor/
│       ├── package.json
│       ├── Runtime/
│       ├── Editor/
│       └── Samples~/
├── README.md
├── LICENSE
├── CHANGELOG.md
├── .gitignore
└── SETUP_GUIDE.md
```

## 📦 Package Installation Instructions

Users can install your package in two ways:

### **Option 1: Git URL (Recommended)**
```
https://github.com/iamhassan1/UniversalEnumEditor.git
```

### **Option 2: Unity Package**
Download from GitHub Releases and import manually.

## 🎉 You're Done!

Your Universal Enum Editor is now:
- ✅ Available on GitHub
- ✅ Installable via Git URL
- ✅ Professionally documented
- ✅ Ready for the Unity community

## 🔄 Updating the Package

When you make updates:

1. Update version in `package.json`
2. Update `CHANGELOG.md`
3. Commit and push changes
4. Create a new GitHub release

## 📞 Support

- **Issues**: Use GitHub Issues
- **Discussions**: Use GitHub Discussions
- **Documentation**: README.md and samples

---

**Happy coding! 🚀**
