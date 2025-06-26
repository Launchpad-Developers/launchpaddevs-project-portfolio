<p align="center">
  <img src="./screenshots/app-icon.png" alt="App Icon" width="120" />
</p>

# EM3 Admin Mobile

## 🔹 Overview

EM3 Admin Mobile is a cross-platform mobile application developed as part of the EM3 event management system. It enables on-the-ground support at large-scale events, empowering event staff to assist patrons directly through staff-facing tools, while also offering a secure **kiosk mode** for patron self-service help. Designed for fast-paced environments, the app ensures both responsiveness and data integrity — even in low-connectivity scenarios.

This project was developed by **Launchpad Developers Inc** in collaboration with the EM3 product and operations teams.

## 🧑‍💻 Role and Responsibilities

I served as the **Lead Mobile Engineer and Architect** for this project. My work included:

- Designing the app architecture using .NET MAUI (originally Xamarin.Forms)
- Building modular, offline-capable features for patron assistance workflows
- Implementing kiosk mode with secure lock-down and auto-refresh logic
- Integrating Azure B2C authentication with token refresh handling
- Coordinating with backend developers to define API contracts
- Establishing a CI/CD pipeline with staged rollouts and diagnostics

## 🚀 Key Features

- Staff tools for scanning badges, assisting patrons, and submitting reports
- Kiosk mode for unattended self-service help
- Offline operation with background sync
- Biometric login support
- Role-based UI and feature visibility
- Push notifications for urgent updates or location-specific alerts

## 📁 Tech Stack

- **Frontend:** .NET MAUI (originally Xamarin.Forms), C#
- **Backend:** Client-owned RESTful API (ASP.NET Core)
- **Auth:** Azure AD B2C with refresh token support
- **Storage:** SQLite + EF Core for offline-first data sync
- **CI/CD:** Azure DevOps + App Center for QA distribution

## 🧪 Testing & CI/CD

- Unit testing with xUnit and Moq
- CI/CD via Azure DevOps with environment-specific build configs
- App Center integration for internal testing and diagnostics

## 📷 Screenshots

> Screenshots have been redacted where necessary for client privacy and security.  
> See the `/screenshots/` folder for non-sensitive UI examples.

## 📄 Code Snippets

> Code snippets provided in this repo are included **with client permission** and represent key architectural patterns used in the application.  
> See the `/snippets/` folder for examples including:
> - Kiosk Mode Lockdown
> - Token Refresh Workflow
> - Offline Data Queueing

## 📌 Notes

This repository includes **redacted visuals and selected code samples only**. The full application source is proprietary and not publicly available.  
All work was performed by **Launchpad Developers Inc** under contract with the client.

---

_© Launchpad Developers Inc. All rights reserved._
