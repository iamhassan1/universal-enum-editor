# 🎯 Universal Enum Editor for Unity

<div align="center">

![Unity Version](https://img.shields.io/badge/Unity-2019.4+-blue.svg)
![License](https://img.shields.io/badge/License-MIT-green.svg)
![GitHub](https://img.shields.io/badge/GitHub-UniversalEnumEditor-brightgreen.svg)

**Streamline enum management in Unity with a powerful, intuitive editor tool.**

[🚀 Features](#-key-features) • [📦 Installation](#-installation) • [⚡ Quick Start](#-quick-start-guide) • [📚 Documentation](#-documentation)

</div>

---

## 🎯 What is Universal Enum Editor?

Managing enums in large Unity projects can be tedious and time-consuming. This tool solves that by providing a professional, feature-rich interface that lets you discover, edit, and manage any enum in your project from one clean window.

**Stop wasting time searching through scripts** - find and edit enums instantly with smart discovery and validation.

---

## ✨ Key Features

### 🔍 **Smart Script Discovery**
- **Automatic scanning** of your entire project for enum-containing scripts
- **Lightning-fast background scanning** that never freezes Unity
- **Advanced filtering** by script name, folder path, or assembly
- **Real-time search** across all discovered scripts

### ✏️ **Direct Enum Editing**
- **Add, remove, and rename** enum values instantly
- **Real-time validation** prevents naming conflicts and syntax errors
- **Safe file modification** - only touches enum blocks, never other code
- **Undo support** through Unity's standard undo system

### 🛡️ **Professional & Reliable**
- **Built-in validation** catches errors before saving
- **Smart error handling** with helpful messages
- **Drag & drop** script assignment
- **Professional UI** that integrates seamlessly with Unity

### 🎯 **Enhanced Productivity**
- **Search and filter** enum values in real-time
- **Alphabetical sorting** with one click
- **Import/export** enum lists from text files
- **Multi-enum support** for scripts containing multiple enums

---

## 🚀 Installation

### Option 1: Install from Git URL (Recommended)
1. Open Unity Package Manager: `Window > Package Manager`
2. Click the **+** button → **Add package from git URL**
3. Paste this URL: `https://github.com/iamhassan1/universal-enum-editor.git`
4. Click **Add**

### Option 2: Download Unity Package
1. Download the latest `UniversalEnumEditor.unitypackage` from [Releases](https://github.com/iamhassan1/UniversalEnumEditor/releases)
2. Import: `Assets > Import Package > Custom Package...`
3. Select the downloaded package

---

## ⚡ Quick Start Guide

### 1. **Open the Editor**
Navigate to `Tools > Universal Enum Editor` in Unity's menu bar.

### 2. **Select a Script**
Choose your enum script using either method:
- **Drag & Drop**: Drag any C# script from the Project window
- **Script Finder**: Click "Click to Find Script" to browse all enum-containing scripts

### 3. **Edit Your Enums**
- **Add values**: Type in the "Add New Value" field and click the + button
- **Remove values**: Click the - button next to any enum value
- **Rename values**: Click on any value and edit it directly
- **Search values**: Use the search bar to filter large enum lists
- **Sort values**: Toggle "Sort A-Z" for alphabetical ordering

### 4. **Save Changes**
Click "Update Enums" to write changes back to your C# file. The tool validates everything before saving.

---

## 📚 Documentation

### **Advanced Features**

#### **Import/Export**
- **Import**: Load enum values from a .txt file (one value per line)
- **Export**: Save current enum values to a .txt file for backup or sharing

#### **Multi-Enum Scripts**
If your script contains multiple enums, select which one to edit from the dropdown.

#### **Smart Validation**
- Prevents duplicate enum names
- Validates C# naming conventions
- Shows helpful error messages
- Disables save button when errors exist

#### **Assembly Filtering**
Filter scripts by assembly to focus on specific parts of your project.

### **Keyboard Shortcuts**
- **Enter**: Add new enum value
- **Delete**: Remove selected enum value
- **Ctrl+F**: Focus search field

---

## 🎮 Sample Usage

The package includes sample scripts demonstrating various enum scenarios:

- **Basic Enums**: Simple enum definitions
- **Complex Enums**: Enums with custom values and attributes
- **Multiple Enums**: Scripts containing several enums

Access samples via Package Manager → Samples → Import "Basic Usage Example"

---

## 🔧 Requirements

- **Unity 2019.4** or newer
- **Any Unity project** with C# scripts containing enums
- **No additional dependencies** required

---

## 🐛 Troubleshooting

### **Common Issues**

**Q: Tool doesn't find my enums?**
A: Make sure your enums are public and in C# scripts. Try refreshing the script database.

**Q: Can't save changes?**
A: Check for validation errors (duplicates, invalid names). The tool prevents invalid saves.

**Q: Script scanning is slow?**
A: First scan may take time. Subsequent scans are cached and much faster.

### **Getting Help**
- Check the [Issues page](https://github.com/iamhassan1/UniversalEnumEditor/issues) for known problems
- Open a new issue if you find a bug
- Review the [Documentation](https://github.com/iamhassan1/UniversalEnumEditor/blob/main/README.md)

---

## 🤝 Contributing

We welcome contributions! Here's how you can help:

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/AmazingFeature`)
3. **Commit** your changes (`git commit -m 'Add some AmazingFeature'`)
4. **Push** to the branch (`git push origin feature/AmazingFeature`)
5. **Open** a Pull Request

### **Development Setup**
1. Clone the repository
2. Open in Unity 2019.4+
3. The tool will be available under `Tools > Universal Enum Editor`

---

## 📄 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

**MIT License** means:
- ✅ Free to use in personal projects
- ✅ Free to use in commercial projects
- ✅ Free to modify and distribute
- ✅ Attribution appreciated but not required

---

## 🙏 Acknowledgments

- **Unity Technologies** for the amazing Unity platform
- **Open Source Community** for inspiration and best practices
- **All Contributors** who help improve this tool

---

## 📞 Contact

- **GitHub**: [@iamhassan1](https://github.com/iamhassan1)
- **Issues**: [GitHub Issues](https://github.com/iamhassan1/UniversalEnumEditor/issues)
- **Discussions**: [GitHub Discussions](https://github.com/iamhassan1/UniversalEnumEditor/discussions)

---

<div align="center">

**Made with ❤️ for the Unity Community**

[⭐ Star this repo](https://github.com/iamhassan1/UniversalEnumEditor) • [🐛 Report issues](https://github.com/iamhassan1/UniversalEnumEditor/issues) • [📖 View documentation](https://github.com/iamhassan1/UniversalEnumEditor/blob/main/README.md)

</div>
