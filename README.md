# WeighForce

WeighForce is a distributed weighbridge integration and logistics tracking system designed to operate across multiple locations and hardware types. It enables real-time capture, synchronization, and tracking of truck movements across the supply chain.

---

## 🌍 Overview

WeighForce connects to weighbridge hardware at different sites and standardizes data capture regardless of device type or communication protocol. Each site runs a local instance of the system, which synchronizes with a central server to provide unified visibility across operations.

The system supports end-to-end tracking of trucks and materials as they move through multiple weighbridges across regions and countries.

---

## 🎯 Key Objectives

- Integrate with diverse weighbridge hardware (RS-232, Ethernet, USB)
- Standardize weight capture across devices and configurations
- Enable real-time and historical tracking of truck movements
- Synchronize data across multiple locations
- Provide operational visibility across the logistics chain

---

## ⚙️ Core Features

### ⚖️ Hardware-Agnostic Integration
- Supports multiple communication methods:
  - RS-232 (serial)
  - Ethernet (TCP/IP)
  - USB interfaces
- Configurable to adapt to different baud rates, frequencies, and device protocols
- Abstracted hardware layer for consistent data ingestion

---

### ⚖️ Weighbridge Operations (Core Functionality)

- Supports standard weighbridge workflows:
  - **First weight (Gross)** → vehicle loaded
  - **Second weight (Tare)** → vehicle empty
  - **Net weight calculation** → Gross - Tare

- Handles:
  - inbound and outbound weighing
  - automatic net weight computation
  - validation of completed weighbridge transactions

- Ensures consistent handling of weighment cycles across all sites

---

### 🖥️ Local Weighbridge Operation
- Runs on a local machine connected to the weighbridge
- Captures:
  - Gross weight
  - Tare weight
  - Net weight
- Operates independently even with limited connectivity

---

### 🔄 Centralized Synchronization
- Local instances sync to a central server
- Ensures data consistency across sites
- Supports multi-location operations

---

### 🚛 Truck Journey Tracking
Track movement of trucks across multiple weighbridges:

Example flow:
- Port entry (e.g., Mozambique)
- Warehouse intake
- Cross-border transfer (e.g., Malawi)
- Final distribution (retail/shop)

Each weighbridge event contributes to a unified journey record.

---

### ⚠️ Cross-Weighbridge Validation & Loss Detection

- Compare weights across different weighbridges in the supply chain
- Detect discrepancies between origin and destination

Example:
- Truck leaves Liwonde at **30 MT**
- Arrives at destination at **28 MT**

👉 System flags discrepancy for review

- Enables:
  - loss detection
  - shrinkage monitoring
  - potential fraud identification
  - operational alerts for weighbridge technicians

---

### 📊 Multi-Site Visibility
- View activity across all weighbridges
- Monitor throughput and movements
- Track product flow across regions

---

## 🧱 Architecture


[Weighbridge Hardware]
↓
[Local WeighForce Instance]
↓
[Central Server]
↓
[UI / Reporting Layer]


- Local nodes handle hardware interaction  
- Central server aggregates and synchronizes data  
- Frontend provides visibility and control  

---

## 🧪 Example Use Case

1. Truck is loaded at port (Mozambique)
2. First weighbridge captures gross weight
3. Truck moves through warehouse and logistics chain
4. Second weighbridge captures tare or arrival weight
5. System compares values across locations
6. Any discrepancies are flagged for investigation

👉 Entire journey is recorded, validated, and auditable

---

## 🛠️ Tech Stack

- Backend: C# (.NET)
- Frontend: Vue.js
- Database: MySQL
- Hardware Integration: Serial (RS-232), TCP/IP, USB

---

## 🔐 Design Considerations

- Works in low-connectivity environments
- Local-first operation with sync capability
- Flexible hardware configuration
- Extensible for additional integrations

---

## 🔮 Future Enhancements

- GPS integration for live truck tracking
- Automated anomaly detection (weight discrepancies)
- Integration with ERP / inventory systems
- Advanced analytics and reporting dashboards

---

## 👤 Author

Developed as part of enterprise logistics and agricultural operations, supporting multi-country supply chain tracking and weighbridge integration.

---

## 📜 License

AGPL-3.0
