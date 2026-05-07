"""
JPI Express Web — Portfolio Diagram Generator
Generates all 7 architecture/reference diagrams as PNG files.
"""

import matplotlib
matplotlib.use("Agg")
import matplotlib.pyplot as plt
import matplotlib.patches as mpatches
from matplotlib.patches import FancyBboxPatch, FancyArrowPatch
import matplotlib.patheffects as pe
import numpy as np
import os

OUT = os.path.dirname(os.path.abspath(__file__))

# ── Palette ──────────────────────────────────────────────────────────────────
BG      = "#F8FAFC"
BLUE    = "#2563EB"
BLUE_LT = "#DBEAFE"
GREEN   = "#16A34A"
GREEN_LT= "#DCFCE7"
ORANGE  = "#EA580C"
ORANGE_LT="#FEF3C7"
PURPLE  = "#7C3AED"
PURPLE_LT="#EDE9FE"
GRAY    = "#64748B"
GRAY_LT = "#F1F5F9"
SLATE   = "#1E293B"
RED     = "#DC2626"
RED_LT  = "#FEE2E2"
TEAL    = "#0891B2"
TEAL_LT = "#CFFAFE"

TITLE_FS  = 14
HEAD_FS   = 10
BODY_FS   = 8.5
SMALL_FS  = 7.5

def save(fig, name):
    path = os.path.join(OUT, name)
    fig.savefig(path, dpi=150, bbox_inches="tight", facecolor=BG)
    plt.close(fig)
    print(f"  ✓ {name}")

def box(ax, x, y, w, h, label, sublabel=None,
        fc=BLUE_LT, ec=BLUE, lw=1.5, fs=BODY_FS, bold=False):
    rect = FancyBboxPatch((x, y), w, h,
                          boxstyle="round,pad=0.02",
                          facecolor=fc, edgecolor=ec, linewidth=lw, zorder=2)
    ax.add_patch(rect)
    weight = "bold" if bold else "normal"
    cy = y + h / 2 + (0.07 if sublabel else 0)
    ax.text(x + w/2, cy, label, ha="center", va="center",
            fontsize=fs, color=SLATE, fontweight=weight, zorder=3)
    if sublabel:
        ax.text(x + w/2, y + h/2 - 0.12, sublabel, ha="center", va="center",
                fontsize=SMALL_FS, color=GRAY, zorder=3)

def arrow(ax, x1, y1, x2, y2, color=GRAY, lw=1.2, style="->"):
    ax.annotate("", xy=(x2, y2), xytext=(x1, y1),
                arrowprops=dict(arrowstyle=style, color=color,
                                lw=lw, connectionstyle="arc3,rad=0.0"),
                zorder=1)

def section_label(ax, x, y, text, color=GRAY):
    ax.text(x, y, text, fontsize=SMALL_FS, color=color,
            fontstyle="italic", va="top")

# ─────────────────────────────────────────────────────────────────────────────
# 01 — Solution Architecture
# ─────────────────────────────────────────────────────────────────────────────
def diagram_01():
    fig, ax = plt.subplots(figsize=(13, 8), facecolor=BG)
    ax.set_facecolor(BG)
    ax.set_xlim(0, 13); ax.set_ylim(0, 8)
    ax.axis("off")

    fig.suptitle("JPI Express Web — Solution Architecture",
                 fontsize=TITLE_FS, fontweight="bold", color=SLATE, y=0.97)

    # ── swimlane backgrounds ──
    for lane, (yb, yt, label, fc) in enumerate([
        (0.2, 1.8,  "Infrastructure & Workers", "#FFF7ED"),
        (1.9, 3.6,  "Data & Schema Management",  "#F0FDF4"),
        (3.7, 5.9,  "Frontend",                  "#EFF6FF"),
        (6.0, 7.8,  "API Host (.NET 9)",          "#F5F3FF"),
    ]):
        ax.add_patch(mpatches.FancyBboxPatch(
            (0.3, yb), 12.4, yt - yb,
            boxstyle="round,pad=0.05", facecolor=fc, edgecolor="#E2E8F0",
            linewidth=1, zorder=0))
        ax.text(0.55, yb + 0.12, label, fontsize=SMALL_FS,
                color=GRAY, fontstyle="italic", va="bottom")

    # ── API Host row ──
    box(ax, 0.6, 6.2, 4.0, 1.2, "JPI.Express.Api",
        ".NET 9 · REST + Blazor WASM host", fc="#EDE9FE", ec=PURPLE, bold=True)
    box(ax, 5.0, 6.2, 3.2, 1.2, "ServiceDefaults",
        "Shared DI / config defaults", fc=GRAY_LT, ec=GRAY)
    box(ax, 8.6, 6.2, 3.8, 1.2, "JPI.Express.Api.Tests",
        "xUnit · Testcontainers (SQL Server)", fc=GRAY_LT, ec=GRAY)

    # ── Frontend row ──
    box(ax, 0.6, 4.0, 4.5, 1.5, "AngularFrontend",
        "Angular 18 · 550 TS files · @ngneat/elf\nserved from API wwwroot/",
        fc=ORANGE_LT, ec=ORANGE)
    box(ax, 5.5, 4.0, 4.5, 1.5, "BlazorWebApp.Client",
        "Blazor WASM · 129 Razor components\nTailwind CSS · net9.0",
        fc=BLUE_LT, ec=BLUE)
    box(ax, 10.4, 4.3, 2.2, 0.8, "blazor-holders/\n(interop layer)",
        fc="#FEF9C3", ec="#CA8A04", fs=SMALL_FS)

    # ── Data row ──
    box(ax, 0.6, 2.1, 4.5, 1.2, "JPI.Express.EntityFramework",
        "18 DbContexts · EF Core 9 migrations", fc=GREEN_LT, ec=GREEN)
    box(ax, 5.5, 2.1, 4.5, 1.2, "JPI.Express.DatabaseManagement",
        "Liquibase SQL + migration tooling", fc=GREEN_LT, ec=GREEN)
    box(ax, 10.4, 2.1, 2.2, 1.2, "SQL Server\n(single schema\nmulti-tenant)",
        fc=GRAY_LT, ec=GRAY, fs=SMALL_FS)

    # ── Infrastructure row ──
    box(ax, 0.6, 0.4, 5.5, 1.1, "JPI.Express.Workers.S3ToAzureStorageMigrator",
        "BackgroundService · queue-based · live migration", fc=RED_LT, ec=RED)
    box(ax, 6.5, 0.4, 3.0, 1.1, "Azure Blob Storage",
        "migration target", fc=TEAL_LT, ec=TEAL)
    box(ax, 9.9, 0.4, 2.7, 1.1, "AWS S3 / MinIO",
        "migration source", fc=GRAY_LT, ec=GRAY)

    # ── arrows ──
    arrow(ax, 4.6, 6.8, 5.0, 6.8)          # api → servicedefaults
    arrow(ax, 4.6, 5.0, 5.5, 5.0)          # angular → blazor (interop)
    arrow(ax, 10.4, 5.2, 9.95, 5.0, color=ORANGE)  # interop from angular
    arrow(ax, 2.85, 6.2, 2.85, 5.5)        # api → angular
    arrow(ax, 7.75, 6.2, 7.75, 5.5)        # api → blazor
    arrow(ax, 2.85, 4.0, 2.85, 3.3)        # frontend → ef
    arrow(ax, 7.75, 4.0, 7.75, 3.3)        # frontend → ef
    arrow(ax, 5.0, 2.7, 5.5, 2.7)          # ef → dbmgmt
    arrow(ax, 10.4, 2.7, 10.4, 2.7)
    arrow(ax, 9.95, 2.7, 10.4, 2.7, color=GREEN) # dbmgmt → sql
    arrow(ax, 3.35, 2.1, 3.35, 1.5)        # ef → sql (down)
    arrow(ax, 6.1, 0.95, 6.5, 0.95)        # worker → azure
    arrow(ax, 9.5, 0.95, 9.9, 0.95)        # azure ← s3

    ax.text(6.5, 7.85, "← REST API (v1/ prefix) + Blazor WASM served via API host",
            fontsize=SMALL_FS, color=GRAY, ha="left")
    ax.text(6.5, 3.55, "313 Repositories · shared CrudRepository base",
            fontsize=SMALL_FS, color=GREEN, ha="left")

    save(fig, "jpi-express-web-01.png")


# ─────────────────────────────────────────────────────────────────────────────
# 02 — Work Order Lifecycle
# ─────────────────────────────────────────────────────────────────────────────
def diagram_02():
    fig, ax = plt.subplots(figsize=(13, 7.5), facecolor=BG)
    ax.set_facecolor(BG)
    ax.set_xlim(0, 13); ax.set_ylim(0, 7.5)
    ax.axis("off")

    fig.suptitle("JPI Express Web — Work Order Lifecycle",
                 fontsize=TITLE_FS, fontweight="bold", color=SLATE, y=0.97)

    # ── top-level job hierarchy ──
    hier = [
        (1.0, 6.2, 2.2, 0.8, "JOB",        "#7C3AED", "#EDE9FE"),
        (4.0, 6.2, 2.2, 0.8, "PLAN",        "#2563EB", "#DBEAFE"),
        (7.0, 6.2, 2.2, 0.8, "WORK ORDER",  "#EA580C", "#FEF3C7"),
        (10.0,6.2, 2.2, 0.8, "STAGE",       "#16A34A", "#DCFCE7"),
    ]
    for x, y, w, h, lbl, ec, fc in hier:
        box(ax, x, y, w, h, lbl, fc=fc, ec=ec, bold=True, fs=HEAD_FS)

    for x1 in [3.2, 6.2, 9.2]:
        arrow(ax, x1, 6.6, x1 + 0.8, 6.6, color=GRAY, lw=1.5)

    # ── 5 work order type boxes ──
    wo_types = [
        (0.4,  4.2, "Permit WO",   PURPLE,  PURPLE_LT, "Permit tracking\n+ utility locates"),
        (2.9,  4.2, "Basic WO",    BLUE,    BLUE_LT,   "Standard field\nwork order"),
        (5.4,  4.2, "Custom WO",   ORANGE,  ORANGE_LT, "Flexible scope\n+ custom fields"),
        (7.9,  4.2, "Setup WO",    GREEN,   GREEN_LT,  "Site/job setup\nwork order"),
        (10.4, 4.2, "Wiring WO",   TEAL,    TEAL_LT,   "Electrical\nwiring work"),
    ]
    for x, y, lbl, ec, fc, sub in wo_types:
        box(ax, x, y, 2.2, 1.4, lbl, sub, fc=fc, ec=ec, bold=True, fs=BODY_FS)
        arrow(ax, x + 1.1, 6.2, x + 1.1, 5.6, color=ec, lw=1.2)

    ax.text(6.5, 3.9, "All types share: UpdateWorkOrder · Timer · Materials · Payments · Attachments · Contracts · History",
            ha="center", va="center", fontsize=SMALL_FS, color=GRAY,
            bbox=dict(boxstyle="round,pad=0.3", fc=GRAY_LT, ec="#CBD5E1", lw=1))

    # ── per-WO tracked items ──
    tracked = [
        (0.4,  1.2, "Field Crew\nTimer",         BLUE_LT,    BLUE),
        (2.9,  1.2, "Materials\nList",            ORANGE_LT,  ORANGE),
        (5.4,  1.2, "Payment\nRecords",           GREEN_LT,   GREEN),
        (7.9,  1.2, "Digital\nContracts",         PURPLE_LT,  PURPLE),
        (10.4, 1.2, "Attachments +\nUtility Locates", TEAL_LT, TEAL),
    ]
    ax.text(6.5, 2.85, "Per Work Order — Tracked Independently, Rolled Up Per WO",
            ha="center", fontsize=SMALL_FS, color=GRAY, fontstyle="italic")
    for x, y, lbl, fc, ec in tracked:
        box(ax, x, y, 2.2, 1.4, lbl, fc=fc, ec=ec, fs=BODY_FS)

    for i, (x, _, _, _, ec) in enumerate(tracked):
        wo_x = wo_types[i][0]
        arrow(ax, wo_x + 1.1, 4.2, x + 1.1, 2.6, color=ec, lw=1.0)

    ax.text(6.5, 0.25, "Quote → Active Bid → Job  ·  Jobs contain multiple Plans  ·  Plans contain multiple Work Orders across Stages",
            ha="center", fontsize=SMALL_FS, color=GRAY)

    save(fig, "jpi-express-web-02.png")


# ─────────────────────────────────────────────────────────────────────────────
# 03 — Angular → Blazor Migration Map
# ─────────────────────────────────────────────────────────────────────────────
def diagram_03():
    fig, ax = plt.subplots(figsize=(11, 7), facecolor=BG)
    ax.set_facecolor(BG)
    ax.axis("off")

    fig.suptitle("JPI Express Web — Angular → Blazor Migration Progress",
                 fontsize=TITLE_FS, fontweight="bold", color=SLATE, y=0.97)

    # overall bar at top
    inner = fig.add_axes([0.08, 0.78, 0.84, 0.10])
    inner.set_facecolor(BG)
    inner.set_xlim(0, 100); inner.set_ylim(0, 1)
    inner.axis("off")
    inner.barh(0.5, 30, height=0.6, color=BLUE, left=0, zorder=2)
    inner.barh(0.5, 70, height=0.6, color=ORANGE_LT, left=30, zorder=2,
               edgecolor=ORANGE, linewidth=0.5)
    inner.text(15, 0.5, "Blazor WASM  ~30%", ha="center", va="center",
               fontsize=HEAD_FS, color="white", fontweight="bold")
    inner.text(65, 0.5, "Angular 18  ~70%", ha="center", va="center",
               fontsize=HEAD_FS, color=ORANGE, fontweight="bold")
    inner.set_title("Overall Frontend Migration State (by file count: 129 Razor vs 550 TypeScript)",
                    fontsize=SMALL_FS, color=GRAY, pad=4)

    # domain-level breakdown
    domains = [
        # (domain, blazor%, angular%, notes)
        ("Project Management\n(208 files)", 25, 75, "Largest domain · active in both"),
        ("Supply Chain\n(204 files)",        20, 80, "2nd largest · mostly Angular"),
        ("Sales\n(122 files)",               35, 65, "Quote→Bid→Job pipeline"),
        ("IAM\n(105 files)",                 40, 60, "Auth/roles partially migrated"),
        ("HR\n(36 files)",                   45, 55, "Smaller domain · faster migration"),
        ("CRM\n(31 files)",                  50, 50, "Roughly at parity"),
        ("Enterprise / Shared\n(est.)",      60, 40, "Layout + shared components"),
    ]

    ax2 = fig.add_axes([0.08, 0.06, 0.60, 0.66])
    ax2.set_facecolor(BG)
    ax2.set_xlim(0, 100)
    ax2.set_ylim(-0.5, len(domains) - 0.5)
    ax2.invert_yaxis()
    ax2.axis("off")

    for i, (domain, blazor_pct, ang_pct, note) in enumerate(domains):
        ax2.barh(i, blazor_pct, height=0.55, color=BLUE,
                 left=0, zorder=2, label="Blazor" if i == 0 else "")
        ax2.barh(i, ang_pct, height=0.55, color=ORANGE_LT,
                 left=blazor_pct, zorder=2, edgecolor=ORANGE, linewidth=0.5,
                 label="Angular" if i == 0 else "")
        ax2.text(-1, i, domain, ha="right", va="center",
                 fontsize=SMALL_FS, color=SLATE)
        ax2.text(blazor_pct / 2, i, f"{blazor_pct}%", ha="center", va="center",
                 fontsize=SMALL_FS, color="white", fontweight="bold")
        ax2.text(blazor_pct + ang_pct / 2, i, f"{ang_pct}%", ha="center",
                 va="center", fontsize=SMALL_FS, color=ORANGE)
        ax2.text(101, i, note, ha="left", va="center",
                 fontsize=SMALL_FS - 0.5, color=GRAY)

    # legend + notes
    ax3 = fig.add_axes([0.72, 0.06, 0.22, 0.66])
    ax3.set_facecolor(BG)
    ax3.axis("off")

    notes = [
        ("Migration Pattern", SLATE, "bold"),
        ("Strangler-fig strategy —", GRAY, "normal"),
        ("Blazor components embedded", GRAY, "normal"),
        ("in Angular via web component", GRAY, "normal"),
        ("wrappers (blazor-holders/)", BLUE, "normal"),
        ("", GRAY, "normal"),
        ("Angular State", SLATE, "bold"),
        ("@ngneat/elf reactive store", GRAY, "normal"),
        ("Angular Material 18", GRAY, "normal"),
        ("Kept at v18 to avoid", GRAY, "normal"),
        ("debt freeze during migration", GRAY, "normal"),
        ("", GRAY, "normal"),
        ("Blazor State", SLATE, "bold"),
        ("Tailwind CSS (not Material)", BLUE, "normal"),
        ("Mirrors API domain structure", GRAY, "normal"),
        ("4 layout types (multi-context)", GRAY, "normal"),
        ("", GRAY, "normal"),
        ("CI/CD Note", SLATE, "bold"),
        ("Angular build decoupled from", RED, "normal"),
        ("standard pipeline — Blazor", RED, "normal"),
        ("build active in main CI", RED, "normal"),
    ]

    for i, (text, color, weight) in enumerate(notes):
        ax3.text(0.05, 0.97 - i * 0.046, text, transform=ax3.transAxes,
                 fontsize=SMALL_FS, color=color, fontweight=weight, va="top")

    save(fig, "jpi-express-web-03.png")


# ─────────────────────────────────────────────────────────────────────────────
# 04 — CI/CD Pipeline Comparison
# ─────────────────────────────────────────────────────────────────────────────
def diagram_04():
    fig, ax = plt.subplots(figsize=(13, 8), facecolor=BG)
    ax.set_facecolor(BG)
    ax.set_xlim(0, 13); ax.set_ylim(0, 8)
    ax.axis("off")

    fig.suptitle("JPI Express Web — CI/CD Pipeline Architecture (Azure Pipelines)",
                 fontsize=TITLE_FS, fontweight="bold", color=SLATE, y=0.97)

    # ── column headers ──
    for x, label, color in [
        (1.5,  "azure-pipelines.yml\n(Standard Build & Publish)", BLUE),
        (5.0,  "azure-hotfix.yml\n(Hotfix Releases)", RED),
        (8.5,  "azure-pipelines-db.yml\n(Database Migrations)", GREEN),
        (11.5, "AngularFrontend/\nazure-pipelines.yaml", ORANGE),
    ]:
        ax.text(x, 7.6, label, ha="center", va="center",
                fontsize=BODY_FS, color=color, fontweight="bold",
                bbox=dict(boxstyle="round,pad=0.3", fc=BG, ec=color, lw=1.5))

    # ── pipeline steps ──
    def pipeline_col(ax, cx, steps):
        y = 6.8
        for step, fc, ec, note in steps:
            w = 2.6
            x = cx - w / 2
            box(ax, x, y - 0.55, w, 0.5, step, fc=fc, ec=ec, fs=SMALL_FS)
            if note:
                ax.text(cx, y - 0.55 - 0.03, note, ha="center", va="top",
                        fontsize=SMALL_FS - 1, color=GRAY, fontstyle="italic")
            if y > 2.0:
                arrow(ax, cx, y - 0.55, cx, y - 0.55 - 0.35, color=ec, lw=1)
            y -= 1.0

    # Standard pipeline
    pipeline_col(ax, 1.5, [
        ("Restore NuGet packages",   BLUE_LT,   BLUE,   None),
        ("Build Blazor WASM",        BLUE_LT,   BLUE,   None),
        ("Build .NET API",           BLUE_LT,   BLUE,   None),
        ("~~Build Angular~~",        GRAY_LT,   GRAY,   "⚠ commented out"),
        ("Publish artifact",         PURPLE_LT, PURPLE, "jpi-express-api"),
        ("PR guard: skip if PR",     GRAY_LT,   GRAY,   "ne(PullRequest)"),
    ])

    # Hotfix pipeline
    pipeline_col(ax, 5.0, [
        ("Restore NuGet (cached)",   RED_LT,  RED,    "NuGet cache active"),
        ("npm install (cached)",     RED_LT,  RED,    "npm cache active"),
        ("Build Angular",            ORANGE_LT,ORANGE, "Full Angular build"),
        ("Build Blazor WASM",        BLUE_LT, BLUE,   None),
        ("Build .NET API",           RED_LT,  RED,    None),
        ("Publish hotfix artifact",  PURPLE_LT,PURPLE, "jpi-express-api-hotfix"),
    ])

    # DB pipeline
    pipeline_col(ax, 8.5, [
        ("Checkout repo",            GREEN_LT, GREEN,  None),
        ("Run EF Core migrations",   GREEN_LT, GREEN,  None),
        ("Run Liquibase SQL scripts",GREEN_LT, GREEN,  "Dual schema strategy"),
        ("Verify migration state",   GREEN_LT, GREEN,  None),
    ])

    # Angular standalone
    pipeline_col(ax, 11.5, [
        ("npm install",              ORANGE_LT, ORANGE, "--legacy-peer-deps"),
        ("npm run build",            ORANGE_LT, ORANGE, "build:local target"),
        ("Copy to wwwroot/",         ORANGE_LT, ORANGE, "Manual step implied"),
    ])

    # ── version & notes row ──
    ax.add_patch(FancyBboxPatch((0.3, 0.2), 12.4, 0.7,
                 boxstyle="round,pad=0.05", facecolor=GRAY_LT,
                 edgecolor="#CBD5E1", linewidth=1))
    ax.text(6.5, 0.55,
            "Semantic versioning: $(major).$(minor).$(revision)   ·   "
            "Hotfix artifact named independently for separate audit trail   ·   "
            "No automated test gate in any pipeline",
            ha="center", va="center", fontsize=SMALL_FS, color=SLATE)

    save(fig, "jpi-express-web-04.png")


# ─────────────────────────────────────────────────────────────────────────────
# 05 — DbContext Domain Map
# ─────────────────────────────────────────────────────────────────────────────
def diagram_05():
    fig, ax = plt.subplots(figsize=(13, 8.5), facecolor=BG)
    ax.set_facecolor(BG)
    ax.set_xlim(0, 13); ax.set_ylim(0, 8.5)
    ax.axis("off")

    fig.suptitle("JPI Express Web — DbContext Domain Map  (18 Bounded Contexts · Single SQL Server Schema)",
                 fontsize=TITLE_FS, fontweight="bold", color=SLATE, y=0.97)

    contexts = [
        # (label, subdomain_count, domain_group, color)
        ("ProjectManagement\nDbContext",   "PM · 208 files",  "Project Management", PURPLE, PURPLE_LT),
        ("WorkOrder\nDbContext",           "WO types · 5",    "Project Management", PURPLE, PURPLE_LT),
        ("JobPlan\nDbContext",             "Jobs / Plans",    "Project Management", PURPLE, PURPLE_LT),
        ("TimerTracking\nDbContext",       "Field crew time", "Project Management", PURPLE, PURPLE_LT),

        ("Sales\nDbContext",               "122 files",       "Sales",              BLUE,   BLUE_LT),
        ("Quote\nDbContext",               "Quote templates", "Sales",              BLUE,   BLUE_LT),

        ("SupplyChain\nDbContext",         "204 files",       "Supply Chain",       GREEN,  GREEN_LT),
        ("Inventory\nDbContext",           "Warehouse / PO",  "Supply Chain",       GREEN,  GREEN_LT),
        ("Vendor\nDbContext",              "Vendor mgmt",     "Supply Chain",       GREEN,  GREEN_LT),

        ("CRM\nDbContext",                 "31 files",        "CRM",                TEAL,   TEAL_LT),

        ("Identity\nDbContext",            "105 files",       "IAM",                ORANGE, ORANGE_LT),
        ("RolePermission\nDbContext",      "CRUD authz",      "IAM",                ORANGE, ORANGE_LT),

        ("HR\nDbContext",                  "36 files",        "HR",                 RED,    RED_LT),

        ("Document\nDbContext",            "Contracts / PDF", "Documents",          GRAY,   GRAY_LT),
        ("Attachment\nDbContext",          "Azure Blob refs", "Documents",          GRAY,   GRAY_LT),

        ("Audit\nDbContext",               "EF interceptors", "Audit",              "#78716C", "#FDF8F0"),

        ("Notification\nDbContext",        "SendGrid email",  "Notifications",      "#0284C7", "#E0F2FE"),

        ("Enterprise\nDbContext",          "Multi-tenant\norg isolation", "Enterprise", SLATE, "#F8FAFC"),
    ]

    cols = 4
    col_w = 2.9
    row_h = 1.55
    x_start = 0.5
    y_start = 7.0

    for i, (label, sub, group, ec, fc) in enumerate(contexts):
        col = i % cols
        row = i // cols
        x = x_start + col * col_w
        y = y_start - row * row_h
        box(ax, x, y - 1.1, 2.6, 1.0, label, sub,
            fc=fc, ec=ec, fs=SMALL_FS, bold=True)

        # group badge
        ax.text(x + 1.3, y - 0.05, group,
                ha="center", va="center",
                fontsize=SMALL_FS - 1.5, color=ec,
                bbox=dict(boxstyle="round,pad=0.15", fc="white", ec=ec, lw=0.8))

    # SQL Server at bottom
    box(ax, 2.0, 0.15, 9.0, 0.6,
        "SQL Server  —  Single shared schema with tenant column filtering  (TenantId FK on all domain tables)",
        fc=GRAY_LT, ec=GRAY, bold=False, fs=SMALL_FS)

    # arrows from each context down to SQL (representative)
    for col in range(cols):
        x = x_start + col * col_w + 1.3
        ax.annotate("", xy=(6.5, 0.75), xytext=(x, 0.9),
                    arrowprops=dict(arrowstyle="->", color="#CBD5E1",
                                    lw=0.8, connectionstyle="arc3,rad=0.0"))

    save(fig, "jpi-express-web-05.png")


# ─────────────────────────────────────────────────────────────────────────────
# 06 — Security Model
# ─────────────────────────────────────────────────────────────────────────────
def diagram_06():
    fig, ax = plt.subplots(figsize=(13, 6.5), facecolor=BG)
    ax.set_facecolor(BG)
    ax.set_xlim(0, 13); ax.set_ylim(0, 6.5)
    ax.axis("off")

    fig.suptitle("JPI Express Web — Authentication & Multi-Tenant Security Model",
                 fontsize=TITLE_FS, fontweight="bold", color=SLATE, y=0.97)

    # ── flow: left to right ──
    stages = [
        (0.4,  3.0, 2.0, 1.8, "Client Request",
         "Bearer JWT token\nin Authorization header",
         GRAY_LT, GRAY),
        (2.9,  3.0, 2.2, 1.8, "AppAuthentication\nHandler",
         "Custom JWT override\n11-min TTL\nAzure AD / OIDC path",
         ORANGE_LT, ORANGE),
        (5.6,  3.0, 2.2, 1.8, "JWT Claims\nExtracted",
         "tenantId (long)\ntenantsubdomain\nemployee-level claims",
         BLUE_LT, BLUE),
        (8.3,  3.0, 2.2, 1.8, "SecurityContext\n(injected service)",
         "TenantId, UserId\ninjected into all\ndomain services",
         PURPLE_LT, PURPLE),
        (10.9, 3.0, 1.8, 1.8, "Domain\nServices",
         "Query scoping\nvia SecurityContext\n(service-layer)",
         GREEN_LT, GREEN),
    ]

    for x, y, w, h, label, sub, fc, ec in stages:
        box(ax, x, y, w, h, label, sub, fc=fc, ec=ec, bold=True, fs=BODY_FS)

    for i in range(len(stages) - 1):
        x1 = stages[i][0] + stages[i][2]
        x2 = stages[i+1][0]
        cy = stages[i][1] + stages[i][3] / 2
        arrow(ax, x1, cy, x2, cy, color=SLATE, lw=1.5)

    # ── CRUD authz box below ──
    box(ax, 2.9, 1.2, 7.2, 1.3,
        "CRUD Operation-Level Authorization",
        "OperationAuthorizationRequirement per resource  ·  Create / Read / Update / Delete\n"
        "Applied at controller level via [Authorize] + [AllowAnonymous]",
        fc=ORANGE_LT, ec=ORANGE, fs=BODY_FS)

    arrow(ax, 4.0, 3.0, 4.0, 2.5, color=ORANGE)
    arrow(ax, 9.4, 3.0, 9.4, 2.5, color=ORANGE)

    # ── Azure AD / SSO side note ──
    box(ax, 0.4, 0.3, 2.2, 1.7,
        "Azure AD / OIDC",
        "Enterprise SSO path\nOpenIdConnect\ncustom auth handler",
        fc=BLUE_LT, ec=BLUE)
    arrow(ax, 2.6, 1.1, 2.9, 2.5, color=BLUE, lw=1)

    # ── multi-tenancy note ──
    ax.text(10.9, 1.5,
            "Multi-Tenancy:\nSingle schema,\ntenant column\nfiltering.\nNo EF global\nquery filters\n(tracked debt)",
            ha="left", va="center", fontsize=SMALL_FS, color=RED,
            bbox=dict(boxstyle="round,pad=0.3", fc=RED_LT, ec=RED, lw=1))

    save(fig, "jpi-express-web-06.png")


# ─────────────────────────────────────────────────────────────────────────────
# 07 — Data Access Layer
# ─────────────────────────────────────────────────────────────────────────────
def diagram_07():
    fig, ax = plt.subplots(figsize=(13, 7.5), facecolor=BG)
    ax.set_facecolor(BG)
    ax.set_xlim(0, 13); ax.set_ylim(0, 7.5)
    ax.axis("off")

    fig.suptitle("JPI Express Web — Data Access Layer  (Dual-Path + Legacy CQRS)",
                 fontsize=TITLE_FS, fontweight="bold", color=SLATE, y=0.97)

    # ── top: Controller ──
    box(ax, 4.5, 6.5, 4.0, 0.7, "API Controller (23 controllers  ·  v1/ REST)",
        fc=GRAY_LT, ec=SLATE, bold=True, fs=BODY_FS)

    # ── service layer ──
    box(ax, 4.5, 5.4, 4.0, 0.7, "Domain Service",
        fc=PURPLE_LT, ec=PURPLE, bold=True, fs=BODY_FS)
    arrow(ax, 6.5, 6.5, 6.5, 6.1)

    # ── three paths ──
    paths = [
        # x,  label,             sublabel,               fc,         ec,      status
        (1.0, "Repository",      "313 repos\nCrudRepository base (9,466 lines)", GREEN_LT,  GREEN,  "Modern"),
        (4.5, "MediatR Handler", "IRequest / IRequestHandler\nassembly-wide registration", BLUE_LT,   BLUE,   "Modern"),
        (8.5, "CommandSender /\nQueryExecutor", "Legacy CQRS\n(in-flight → MediatR)", ORANGE_LT, ORANGE, "Legacy"),
    ]

    for x, lbl, sub, fc, ec, status in paths:
        box(ax, x, 3.8, 3.2, 1.2, lbl, sub, fc=fc, ec=ec, bold=True)
        col = GREEN if status == "Modern" else ORANGE
        ax.text(x + 1.6, 5.1, status, ha="center", fontsize=SMALL_FS,
                color="white",
                bbox=dict(boxstyle="round,pad=0.2", fc=col, ec=col, lw=0))

    # arrows from service to paths
    arrow(ax, 5.5, 5.4, 2.6, 5.0, color=GREEN, lw=1.2)
    arrow(ax, 6.5, 5.4, 6.1, 5.0, color=BLUE, lw=1.2)
    arrow(ax, 7.5, 5.4, 10.1, 5.0, color=ORANGE, lw=1.2)

    # ── data layer ──
    box(ax, 1.0, 2.4, 3.2, 1.0, "EF Core 9 DbContext",
        "18 domain-scoped contexts\nLINQ · migrations",
        fc=GREEN_LT, ec=GREEN)
    box(ax, 4.5, 2.4, 3.2, 1.0, "Dapper\nDataSession",
        "~790 lines · raw SQL\ncomplex legacy queries",
        fc=ORANGE_LT, ec=ORANGE)
    box(ax, 8.5, 2.4, 3.2, 1.0, "Dapper\nDataSession",
        "shared with legacy CQRS\n~790 lines",
        fc=ORANGE_LT, ec=ORANGE)

    arrow(ax, 2.6, 3.8, 2.6, 3.4, color=GREEN)
    arrow(ax, 6.1, 3.8, 6.1, 3.4, color=ORANGE)
    arrow(ax, 10.1, 3.8, 10.1, 3.4, color=ORANGE)

    # ── SQL Server ──
    box(ax, 2.5, 0.8, 8.0, 0.9,
        "SQL Server — Single Shared Schema (Multi-Tenant, Row-Level TenantId Filtering)",
        fc=GRAY_LT, ec=SLATE, bold=True, fs=BODY_FS)

    for cx in [2.6, 6.1, 10.1]:
        arrow(ax, cx, 2.4, cx, 1.7, color=SLATE)

    # ── risk callout ──
    ax.text(6.5, 0.3,
            "⚠  EF Core and Dapper operate on the same schema with no shared transaction coordination — "
            "a latent data integrity risk under active remediation",
            ha="center", fontsize=SMALL_FS, color=RED,
            bbox=dict(boxstyle="round,pad=0.3", fc=RED_LT, ec=RED, lw=1))

    save(fig, "jpi-express-web-07.png")


# ─────────────────────────────────────────────────────────────────────────────
if __name__ == "__main__":
    print("Generating JPI Express Web diagrams...")
    diagram_01()
    diagram_02()
    diagram_03()
    diagram_04()
    diagram_05()
    diagram_06()
    diagram_07()
    print("Done.")
