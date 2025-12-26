# NFC ACR122U Reader/Writer

A Windows Forms application for reading and writing NFC cards using the ACR122U NFC reader device.

## ?? Overview

This application provides a simple graphical interface to interact with MIFARE Classic NFC cards through the ACR122U USB NFC reader. It supports real-time card detection, reading, and writing operations on card memory blocks.

## ? Features

- **Real-time Card Detection**: Automatically detects when NFC cards are placed on or removed from the reader
- **Card UID Display**: Shows the unique identifier of the connected card
- **Block Reading**: Read data from any block (1-63) on MIFARE Classic cards
- **Block Writing**: Write text data to any block on the card
- **Visual Status Indicators**: Color-coded connection status (Green = Connected, Red = Disconnected)
- **Thread-Safe Operations**: Ensures smooth UI updates during card operations
- **Comprehensive Logging**: Built-in logging system for debugging and monitoring operations

## ?? Requirements

### Hardware
- **ACR122U NFC Reader** (USB)
- **MIFARE Classic NFC Cards** (or compatible cards)

### Software
- **Windows OS** (Windows 7 or later)
- **.NET Framework 4.7.2** or higher
- **PC/SC Smart Card drivers** (usually included with Windows)

## ?? Dependencies

This project uses the following NuGet packages:
- **SnappyWinscard** (v1.0.0.3) - PC/SC smart card communication library
- **NLog** (v4.7.15) - Logging framework

## ?? Getting Started

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/johanhenningsson4-hash/NFC-ACR-Reader-Writer.git
   cd NFC-ACR-Reader-Writer
   ```

2. **Open the solution**
   - Open `NFC-ACR122U-Reader-Writer.sln` in Visual Studio

3. **Restore NuGet packages**
   - Visual Studio will automatically restore the required packages
   - Or manually: Right-click solution ? Restore NuGet Packages

4. **Build and run**
   - Press F5 or click Start to build and run the application

### Hardware Setup

1. Connect your ACR122U reader to a USB port
2. Wait for Windows to install the drivers automatically
3. Place an NFC card on the reader
4. Launch the application

## ?? Usage

### Reading Card Data

1. Ensure a card is placed on the reader (status indicator should be green)
2. Select the block number you want to read (1-63) using the numeric up/down control
3. Click the **Read** button
4. The data from the selected block will be displayed in the text field

### Writing Card Data

1. Ensure a card is placed on the reader (status indicator should be green)
2. Select the block number you want to write to (1-63)
3. Enter the text you want to write in the input field
4. Click the **Write** button
5. A message box will confirm success or failure

### Viewing Logs

The application automatically logs all operations to help with debugging and monitoring:

- **Log Location**: `logs/` folder in the application directory
- **Daily Logs**: `nfc-reader-YYYY-MM-DD.log` - Contains all operations
- **Error Logs**: `errors-YYYY-MM-DD.log` - Contains only errors and exceptions
- **Log Retention**: Daily logs are kept for 7 days, error logs for 30 days
- **Debug Output**: Logs are also visible in Visual Studio's Output window when debugging

**Log Levels:**
- **Debug**: Detailed operation information (card connections, block reads/writes)
- **Info**: General application events (button clicks, successful operations)
- **Warn**: Non-critical issues (no card detected)
- **Error**: Exceptions and failures

### Understanding the Interface

| Element | Description |
|---------|-------------|
| **Status Bar** | Color indicator at the top (Green = Connected, Red = Disconnected) |
| **CARD** | Displays the card's unique identifier (UID) |
| **Card Status** | Shows the current card connection status |
| **Card Sub Status** | Displays additional status information |
| **Block No** | Numeric selector for choosing which block to read/write (1-63) |
| **Read Field** | Displays data read from the selected block |
| **Write Field** | Input field for data to be written to the card |
| **Reader State** | Shows the current state of the card reader |

## ?? Important Notes

### Security Considerations

- This application uses **default MIFARE keys** (Key A: `FFFFFFFFFFFF` or 0x0)
- It will only work with cards that use factory default keys
- **Not suitable for production use** without implementing proper key management
- Cards with changed sector keys will not be accessible

### Block Structure

- MIFARE Classic 1K cards have 16 sectors
- Each sector has 4 blocks
- Block 0 contains the manufacturer data (usually read-only)
- Sector trailer blocks (every 4th block) contain access keys and should be handled carefully
- Safe blocks for general data: 1, 2, 4, 5, 6, 8, 9, 10, etc.

### Data Limitations

- Each block stores exactly **16 bytes** of data
- Text longer than 16 characters will be truncated
- Text shorter than 16 characters will be padded with null characters
- Special characters may not display correctly depending on encoding

## ??? Project Structure

```
NFC-ACR-Reader-Writer/
??? Program.cs              # Application entry point
??? Form1.cs                # Main form logic and card operations
??? Form1.Designer.cs       # UI designer-generated code
??? Form1.resx              # Form resources
??? App.config              # Application configuration
??? NLog.config             # Logging configuration
??? packages.config         # NuGet package references
??? logs/                   # Log files directory (auto-created)
?   ??? nfc-reader-*.log    # Daily operation logs
?   ??? errors-*.log        # Daily error logs
?   ??? archive/            # Archived logs
??? Properties/
    ??? AssemblyInfo.cs
    ??? Resources.Designer.cs
    ??? Settings.Designer.cs
```

## ?? Technical Details

### Authentication
- Uses **Key Type A** (0x60) for authentication
- Default key: `0x00` (all zeros)
- Authentication is performed automatically before read/write operations

### Thread Safety
- UI updates are handled using `BackgroundWorker` pattern
- Prevents cross-thread exceptions during card state change events

### Card Communication
- Utilizes PC/SC (Personal Computer/Smart Card) standard
- Communicates through the SnappyWinscard library wrapper
- Supports standard APDU (Application Protocol Data Unit) commands

### Logging System
- **Framework**: NLog 4.7.15
- **Log Levels**: Debug, Info, Warn, Error
- **Output Targets**: 
  - File (daily rotation with 7-day retention)
  - Error file (daily rotation with 30-day retention)
  - Debug console (Visual Studio Output window)
- **Thread-Safe**: All logging operations are thread-safe
- **Performance**: Asynchronous logging doesn't block card operations

**Logged Operations:**
- Application startup/shutdown
- Card connection/disconnection events
- Card UID retrieval
- Block read operations (including data)
- Block write operations (including data)
- Authentication attempts
- All errors and exceptions with stack traces

## ?? Troubleshooting

### Card Not Detected
- Ensure the ACR122U reader is properly connected
- Check Device Manager for PC/SC reader drivers
- Try removing and reinserting the card
- Restart the application
- **Check the logs** in the `logs/` folder for detailed error information

### Read/Write Failures
- Verify the card is a MIFARE Classic card
- Ensure the card uses default keys
- Check that you're not trying to write to protected blocks (0, 3, 7, 11, etc.)
- Make sure the card is positioned correctly on the reader
- **Review error logs** for authentication or communication failures

### Application Crashes
- Ensure .NET Framework 4.7.2 is installed
- Check that SnappyWinscard package is properly restored
- Verify Windows has the necessary PC/SC drivers
- **Examine error logs** for exception details and stack traces

### Debugging with Logs

1. Reproduce the issue
2. Navigate to the `logs/` folder in the application directory
3. Open the most recent `nfc-reader-YYYY-MM-DD.log` file
4. Search for ERROR or WARN entries
5. Review the stack traces and error messages
6. For critical errors, check `errors-YYYY-MM-DD.log`

## ?? Known Limitations

- Only supports MIFARE Classic cards with default keys
- No support for MIFARE DESFire or other card types
- Limited error messages for debugging
- No support for custom authentication keys

## ?? Contributing

Contributions are welcome! Feel free to:
- Report bugs
- Suggest new features
- Submit pull requests
- Improve documentation

## ?? License

This project is open source. Please check the repository for license details.

## ?? Resources

- [ACR122U Product Page](https://www.acs.com.hk/en/products/3/acr122u-usb-nfc-reader/)
- [MIFARE Classic Documentation](https://www.nxp.com/docs/en/data-sheet/MF1S50YYX_V1.pdf)
- [SnappyWinscard on NuGet](https://www.nuget.org/packages/SnappyWinscard/)

## ?? Author

Johan Henningsson

## ?? Support

For issues and questions, please use the [GitHub Issues](https://github.com/johanhenningsson4-hash/NFC-ACR-Reader-Writer/issues) page.

---

**? Quick Start**: Connect your ACR122U reader, place a MIFARE Classic card on it, run the application, and start reading/writing!
