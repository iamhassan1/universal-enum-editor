# Changelog

All notable changes to the Universal Enum Editor project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Initial release of Universal Enum Editor
- Smart script discovery and scanning
- Direct enum editing capabilities
- Real-time validation and error checking
- Search and filtering functionality
- Import/export features
- Professional Unity editor integration

## [1.0.0] - 2024-12-19

### Added
- **Core Features**
  - Universal enum editor window accessible via `Tools > Universal Enum Editor`
  - Automatic project scanning for enum-containing scripts
  - Smart script discovery with filtering options
  - Direct enum value editing (add, remove, rename)
  - Real-time validation and error prevention
  - Search and filter functionality for large enum lists
  - Alphabetical sorting capabilities
  - Import/export enum values to/from text files
  - Multi-enum script support
  - Assembly-based filtering
  - Drag & drop script assignment
  - Professional UI with Unity integration

- **Technical Features**
  - Background script scanning that never freezes Unity
  - Safe file modification (only touches enum blocks)
  - Built-in C# naming convention validation
  - Duplicate detection and prevention
  - Undo support through Unity's standard system
  - Persistent script path preferences
  - Error handling with user-friendly messages

- **Documentation**
  - Comprehensive README with installation instructions
  - Quick start guide and troubleshooting
  - Sample scripts demonstrating various use cases
  - Professional package structure for Unity Package Manager

### Technical Details
- **Unity Version Support**: 2019.4+
- **Dependencies**: None (pure Unity editor tool)
- **License**: MIT
- **Package Name**: `com.mhstools.universalenumeditor`
