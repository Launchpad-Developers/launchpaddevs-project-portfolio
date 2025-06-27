<p align="center">
  <img src="./screenshots/app-icon.png" alt="App Icon" width="120" />
</p>

# Facility Fit: Inspections

**Facility Fit: Inspections** is a dynamic mobile app used by healthcare engineering and safety teams to manage on-site inspections and compliance reporting. Designed for flexibility and scalability, the app features dynamic, XML-driven workflows and advanced search/filter tools.

As the sole mobile engineer, I replaced the legacy Cordova implementation with a robust Xamarin.Forms build. The old JavaScript served only as a reference for API structure and data flows — the new app was rebuilt by hand for offline resilience, usability, and modern maintainability.

---

## 📌 **Key Highlights**
- Fully dynamic inspection workflows powered by XML and decision trees.
- Multi-site deployment with **server and campus switching** for different facilities.
- Integrated **QR/barcode scanning** for fast asset or space lookups.
- Flexible search and filtering tools for inspections by date, space group, building, or template.
- Branded, field-friendly UX aligned with the broader Facility Fit suite.
- Full offline mode for areas with poor connectivity.

---

## ⚙️ **Technical Details**
- Xamarin.Forms + Prism.Forms (MVVM)
- RESTful API (ASP.NET 4.5)
- Azure Notification Hub for push
- CI/CD with MS App Center
- Uses ZXing scanner for identifying equipment and inspection stations

---

## 📷 **Screenshots**

<div style="display: flex; gap: 1rem;">
  <img src="/screenshots/inspections-02.png" alt="Login screen" style="border: 2px solid #2A7AE2; border-radius: 4px;">
  <img src="/screenshots/inspections-05.png" alt="Context menu" style="border: 2px solid #2A7AE2; border-radius: 4px;">
  <img src="/screenshots/inspections-07.png" alt="Barcode scanner" style="border: 2px solid #2A7AE2; border-radius: 4px;">
</div>

> See `/screenshots/` folder for more UI examples.

## 🔐 Notes


FacilityFit Inspections is a privately listed enterprise app and cannot be downloaded by the general public.

The repository includes select screenshots and redacted summaries only. Full source is proprietary.

All work was performed by **Launchpad Developers Inc** under contract with Aramark Healthcare leadership.

---

_© Launchpad Developers Inc. All rights reserved._

