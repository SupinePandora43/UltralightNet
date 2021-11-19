namespace UltralightNet
{
	public struct ULCommand
	{
		public ULCommandType command_type;
		public ULGPUState gpu_state;

		/// <remarks>Only used when <see cref="command_type"/> is <see cref="ULCommandType.DrawGeometry"/></remarks>
		public uint geometry_id;
		/// <remarks>Only used when <see cref="command_type"/> is <see cref="ULCommandType.DrawGeometry"/></remarks>
		public uint indices_count;
		/// <remarks>Only used when <see cref="command_type"/> is <see cref="ULCommandType.DrawGeometry"/></remarks>
		public uint indices_offset;
	}
}
