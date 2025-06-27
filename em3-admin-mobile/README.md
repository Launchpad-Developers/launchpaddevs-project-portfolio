<p align="center">
  <img src="./screenshots/app-icon.png" alt="App Icon" width="120" />
</p>

# EM3 Admin Mobile

## 🔹 Leadership & Project Overview

EM3 Admin Mobile is a cross-platform mobile application developed as part of the EM3 event management platform. Its goal: to empower both staff and patrons during high-volume events through intuitive tooling and robust kiosk support.

At Launchpad Developers Inc., we believe great software starts with great leadership. This project wasn’t just about delivering features—it was about architecting with clarity, leading with empathy, and making decisions that scaled with the team.

From field logistics to kiosk security, every implementation decision reflected the need for sustainability, maintainability, and performance under pressure.

## 🧑‍💼 My Role

As the **Lead Mobile Engineer and Architect**, I was responsible for both execution and technical leadership:

- Designed the mobile architecture using .NET MAUI (originally Xamarin.Forms)
- Led development of modular, offline-first workflows for patron support
- Architected kiosk mode with secure lockdown and self-healing refresh logic
- Integrated Azure B2C authentication with token refresh workflows
- Collaborated with backend teams to define clear, maintainable API contracts
- Drove CI/CD implementation with staged QA rollouts and diagnostic telemetry

## 🧭 Leadership Principles in Action

- Built for **clarity over cleverness**, enabling junior devs to contribute quickly
- Chose **offline-first patterns** to ensure field reliability, even in low-connectivity areas
- Ensured **role-based feature access** so different user types had tailored flows
- Practiced **technical empathy**, prioritizing readable code and clear error paths

## 🚀 Key Capabilities

- Badge scanning, patron lookup, and incident reporting for staff
- Self-service kiosk mode with auto-refresh and PIN-locked access
- Offline data capture with conflict-resilient sync
- Biometric login support
- Push notifications scoped by role or geo-location

## 🧰 Tech Stack

- **Frontend:** .NET MAUI (originally Xamarin.Forms), C#
- **Backend:** Client-owned REST API (ASP.NET Core)
- **Auth:** Azure AD B2C with refresh token support
- **Storage:** SQLite + EF Core for disconnected operation
- **CI/CD:** Azure DevOps + App Center

## 🧪 Testing & QA Delivery

- Unit testing with xUnit and Moq
- CI pipelines with environment-specific configs
- App Center distribution for QA and diagnostic feedback

## 📷 Screenshots

<div style="display: flex; gap: 1rem;">
  <img src="screenshot1.png" alt="Assignments Screenshot 1" style="border: 2px solid #2A7AE2; border-radius: 4px;">
  <img src="screenshot2.png" alt="Assignments Screenshot 2" style="border: 2px solid #2A7AE2; border-radius: 4px;">
  <img src="screenshot3.png" alt="Assignments Screenshot 3" style="border: 2px solid #2A7AE2; border-radius: 4px;">
</div>

> See `/screenshots/` folder for more UI examples. Sensitive content has been redacted where necessary.

## 📄 Code Snippets

> Included with client permission. Browse `/snippets/` for patterns like:
> - Kiosk Lockdown Flow
> - Refresh Token Handling
> - Offline Sync Queuing

## 🔐 Notes

This is a privately listed app and cannot be downloaded by the general public.

This repo includes curated, redacted content only. Full source is proprietary.

All work was performed under contract by **Launchpad Developers Inc**.

---

_© Launchpad Developers Inc. All rights reserved._

