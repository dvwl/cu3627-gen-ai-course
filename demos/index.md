---
layout: default
title: Gantt Chart App (C#)
parent: Demo Projects
nav_order: 1
---

# Project Estimation & Gantt Chart Web App (C#)
{: .no_toc }

An ASP.NET Core web application demonstrating project task management, three-point estimation, and automated Mermaid Gantt chart generation.

**Technology Stack**: .NET 8, Entity Framework, SQLite, Mermaid.js  
**Estimated Effort**: 80 hours (2 weeks)  
**Complexity**: Medium  
{: .label .label-blue }

<details open markdown="block">
  <summary>
    Table of contents
  </summary>
  {: .text-delta }
1. TOC
{:toc}
</details>

---

## Project Overview

This demo web application showcases practical implementation of project estimation and task management concepts covered in the course. It includes:

- CRUD operations for project tasks
- Three-point estimation (optimistic, pessimistic, most likely) with PERT calculations
- Task dependency management
- Automated Mermaid Gantt chart generation and visualization
- Critical path identification
- Project timeline visualization

### Key Features

- **Task Management**: Create, edit, update, and delete project tasks
- **Estimation Tools**: Built-in three-point estimation with confidence intervals
- **Dependency Tracking**: Define task relationships and prerequisites
- **Mermaid Integration**: Auto-generate Mermaid Gantt charts from task data
- **Visual Timeline**: Interactive web-based Gantt chart viewer
- **Responsive Design**: Works on desktop and mobile devices

---

## Estimation Breakdown

### Functional Requirements Analysis

| Feature Area | Function Points | Complexity | Estimated Hours |
|--------------|----------------|------------|-----------------|
| Task CRUD Operations | 6 | Medium | 16 |
| Three-Point Estimation | 3 | Low | 8 |
| Task Dependencies | 4 | Medium | 12 |
| Mermaid Chart Generation | 5 | Medium | 16 |
| Web UI (Razor Pages) | 8 | Medium | 20 |
| Data Persistence (EF Core) | 3 | Low | 8 |
| **Total** | **29** | | **80** |

### Application Architecture

```
ProjectEstimator/
├── Controllers/
│   ├── HomeController.cs
│   ├── TasksController.cs
│   └── ProjectsController.cs
├── Models/
│   ├── Project.cs
│   ├── ProjectTask.cs
│   └── ViewModels/
├── Views/
│   ├── Tasks/
│   ├── Projects/
│   └── Shared/
├── Services/
│   ├── EstimationService.cs
│   └── MermaidService.cs
├── Data/
│   └── ApplicationDbContext.cs
└── wwwroot/
    ├── js/
    └── css/
```

### Key Technologies Used

- **ASP.NET Core 8**: Web framework
- **Entity Framework Core**: Data access with SQLite
- **Mermaid.js**: Gantt chart generation and visualization
- **Bootstrap 5**: Responsive UI framework
- **Chart.js**: Additional data visualization

---

## Application Features

### Task Management
- **Create Tasks**: Add new tasks with name, description, and estimates
- **Edit Tasks**: Update task details, estimates, and dependencies
- **Delete Tasks**: Remove tasks and automatically update dependencies
- **List View**: Display all tasks with their key information

### Three-Point Estimation
- **PERT Calculation**: Automatic calculation of expected duration using (O + 4M + P) / 6
- **Standard Deviation**: Calculate uncertainty using (P - O) / 6
- **Confidence Intervals**: Display 68% and 95% confidence ranges
- **Estimation Summary**: Project-level aggregation of estimates

### Task Dependencies
- **Prerequisite Tasks**: Define which tasks must complete before others can start
- **Dependency Validation**: Prevent circular dependencies
- **Critical Path**: Identify tasks that directly impact project completion date
- **Visual Dependencies**: Show task relationships in Mermaid charts

### Mermaid Chart Generation
- **Auto-Generation**: Create Mermaid Gantt chart syntax from task data
- **Live Preview**: Real-time chart rendering using Mermaid.js
- **Export Options**: Copy Mermaid syntax for use in documentation
- **Responsive Viewer**: Charts scale appropriately on different screen sizes

---

## Setup Instructions

### Prerequisites
- Visual Studio Code
- .NET 8.0 SDK
- C# Extension for VS Code

### Installation Steps

1. **Navigate to Demo Directory**
   ```bash
   cd demos/ProjectEstimator
   ```

2. **Restore NuGet Packages**
   ```bash
   dotnet restore
   ```

3. **Create Database**
   ```bash
   dotnet ef database update
   ```

4. **Run Application**
   ```bash
   dotnet run
   ```

5. **Open in Browser**
   Navigate to `http://localhost:5000`

### Development in VS Code
1. Open the `demos/ProjectEstimator` folder in VS Code
2. Press `F5` to start debugging
3. The application will open in your default browser
4. Use breakpoints for debugging the C# code

---

## Estimation Analysis

### Original Estimates vs. Actual

| Phase | Estimated Hours | Notes |
|-------|-----------------|-------|
| Project Setup & Configuration | 4 | ASP.NET Core project template, NuGet packages |
| Data Models & Entity Framework | 8 | ProjectTask, Project models with EF Core setup |
| CRUD Controllers & Views | 20 | Tasks and Projects controllers with Razor views |
| Three-Point Estimation Service | 8 | PERT calculations and confidence intervals |
| Mermaid Chart Generation | 16 | Service to generate Mermaid syntax from tasks |
| UI/UX with Bootstrap | 16 | Responsive design and user interface |
| Testing & Debugging | 8 | Unit tests and integration testing |
| **Total** | **80** | Focused scope for educational demo |

### Key Insights for Web Applications

**Advantages of Web vs Desktop:**
- Cross-platform compatibility (Windows, Mac, Linux)
- No installation required - runs in browser
- Easy sharing and collaboration
- Simpler deployment and updates
- Better integration with online tools (Mermaid, etc.)

**Estimation Considerations:**
- Web UI development often faster with modern frameworks
- Database integration straightforward with EF Core
- Client-side JavaScript adds complexity but provides better UX
- Testing across browsers requires additional effort

---

## Learning Objectives Demonstrated

### Three-Point Estimation
- **PERT Formula**: Implementation of (O + 4M + P) / 6 calculation
- **Confidence Intervals**: 68% and 95% probability ranges
- **Project Aggregation**: Sum of individual task estimates with combined uncertainty

### Task Dependencies
- **Prerequisite Management**: Tasks that must complete before others can start
- **Dependency Validation**: Prevention of circular dependencies in the system
- **Critical Path Identification**: Automated detection of tasks that impact project completion

### Mermaid Integration
- **Chart Generation**: Automatic creation of Mermaid Gantt chart syntax
- **Live Visualization**: Real-time rendering of charts as tasks are modified
- **Export Capability**: Copy generated Mermaid code for use in documentation

### Web Development Best Practices
- **MVC Pattern**: Clean separation of concerns in ASP.NET Core
- **Entity Framework**: Modern data access with code-first approach
- **Responsive Design**: Mobile-friendly interface using Bootstrap
- **RESTful APIs**: Clean controller design for CRUD operations

---

## Extension Exercises

### Beginner Level
1. **Add task status tracking** (Not Started, In Progress, Completed, Blocked)
2. **Implement task filtering** by status, priority, or assignee
3. **Add basic validation** for task dates and estimates

### Intermediate Level
4. **Resource assignment** - assign team members to tasks
5. **Workload visualization** - show resource utilization over time
6. **API endpoints** - create REST API for external integration

### Advanced Level
7. **Monte Carlo simulation** for project completion probability
8. **Real-time updates** using SignalR for collaborative editing
9. **Export functionality** - generate PDF reports or Excel files

---

## Demo Application Structure

The application is located in the `demos/ProjectEstimator/` directory within this repository and includes:

### Key Files
- `Program.cs` - Application entry point and configuration
- `Models/ProjectTask.cs` - Task entity with three-point estimation
- `Models/Project.cs` - Project entity with task collections
- `Controllers/TasksController.cs` - CRUD operations for tasks
- `Services/EstimationService.cs` - Three-point estimation calculations
- `Services/MermaidService.cs` - Mermaid chart generation logic
- `Views/Tasks/` - Razor views for task management
- `wwwroot/js/mermaid-viewer.js` - Client-side Mermaid rendering

### Sample Data
The application includes sample project data to demonstrate:
- Tasks with dependencies (Requirements → Design → Development → Testing)
- Three-point estimates for each task
- Generated Mermaid Gantt chart showing timeline and dependencies
- Critical path highlighting

---

*This demo application illustrates practical application of estimation techniques in a real software project, providing hands-on experience with three-point estimation, critical path analysis, and project scheduling.*
