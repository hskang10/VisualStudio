

using System.Runtime.InteropServices;

namespace SensorNetworkManager_WPF {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Packet {
		public byte type;
		public byte sourceID;
		public byte sourceLevel;
		public byte senderID;
		public byte senderLevel;
		public byte receiverID;

		public Packet(byte type, byte sourceID, byte sourceLevel, byte senderID, byte senderLevel, byte receiverID) {
			this.type = type;
			this.sourceID = sourceID;
			this.sourceLevel = sourceLevel;
			this.senderID = senderID;
			this.senderLevel = senderLevel;
			this.receiverID = receiverID;
		}

		public override string ToString() {
			return type.ToString("X2") + "-" + sourceID.ToString("X2") + "-" + sourceLevel.ToString("X2") + "-" + senderID.ToString("X2") + "-" +
				   senderLevel.ToString("X2") + "-" + receiverID.ToString("X2");
		}
	}

	public enum MessageType {
		Initialization = 0x00,
		Reset = 0x01,
		Sensing = 0x21,
		SensingAck = 0x22,
		Follow = 0xEE
	}

	public enum Id {
		Sink = 0x00,
		Broadcast = 0xFF,
		Center = 0xEE
	}
}