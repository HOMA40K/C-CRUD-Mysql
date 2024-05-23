using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AV
{
	public class ConsoleEntry
	{
		public enum Severity
		{
			Ok,
			Notify,
			Warning,
			Error,
			Critical
		};

		public int ID { get;  }
		public Severity Level { get; }
		public string Message { get; }
		public string Sender { get; }

		public ConsoleEntry (string msg, Severity lvl=Severity.Ok, int identifier=0)
		{
			ID = identifier;
			Message = msg;
			Sender = "";
			Level = lvl;
		}

		public ConsoleEntry(string sender, string msg, Severity lvl = Severity.Ok, int identifier = 0)
		{
			ID = identifier;
			Message = msg;
			Sender = sender??"";
			Level = lvl;
		}
	};

	public class MsgConsoleEventArgs : EventArgs
	{
		public enum EventType
		{
			MessagePushed,
			MessagesPushed,
			Overflow
		}

		public int StartID { get; }
		public int LastID { get; }

		public MsgConsoleEventArgs(EventType t, int start, int finish)
		{
			StartID = start;
			LastID = finish;
		}
	}

	public class MsgConsole
	{
		/* Fields, Properties */
		private int EntriesMax = 1000;
		private List<ConsoleEntry> Entries=new List<ConsoleEntry>();
		//private bool Overflow=false;

		private int DisplayMarker = -1;
		/* Fields, Properties - END */

		/* Event Stuff */
		public event EventHandler<MsgConsoleEventArgs> Message_Pushed;
		public event EventHandler<MsgConsoleEventArgs> Messages_Pushed;
		public event EventHandler<MsgConsoleEventArgs> Overflow;

		protected virtual void OnMessage_Pushed(MsgConsoleEventArgs e) => Message_Pushed?.Invoke(this, e);
		protected virtual void OnMessages_Pushed(MsgConsoleEventArgs e) => Messages_Pushed?.Invoke(this, e);
		protected virtual void OnOverflow(MsgConsoleEventArgs e) => Overflow?.Invoke(this, e);
		/* Event Stuff - END */

		/* Methods */
		public void SetDisplayMarker(int id)
		{
			int index = Entries.FindIndex(x => x.ID == id);
			if (index == -1) return;
			//List<ConsoleEntry>.Enumerator e=Entries.GetEnumerator();
			DisplayMarker = id;
		}

		public int GetDisplayMarker() => DisplayMarker;

		public bool IsEmpty() => Entries.Count == 0;

		public int GetMessageCount() => Entries.Count;
		public int GetLatestID() => Entries.Count == 0 ? 0 : Entries.Last().ID;

		public ConsoleEntry GetMessageAtIndex(int index) => index > Entries.Count - 1 ? new ConsoleEntry("Wrong message index", ConsoleEntry.Severity.Error, -1) : Entries[index];
		public ConsoleEntry GetMessage(int id) => Entries.Find(x => x.ID == id) ?? new ConsoleEntry("Messages with this ID not found", ConsoleEntry.Severity.Critical);
		public ConsoleEntry GetLatestMessage() => Entries.Count == 0 ? new ConsoleEntry("No console entries", ConsoleEntry.Severity.Error, -1) : Entries.Last();

		public List<ConsoleEntry> GetAllMessages() => new List<ConsoleEntry>(Entries);
		public List<ConsoleEntry> GetAllMessagesAfterID(int id)
		{
			int index = GetIndexClosestAfterID(id);
			List<ConsoleEntry> result = index == -1 ? new List<ConsoleEntry>() : new List<ConsoleEntry>(Entries.GetRange(index, Entries.Count-index-1));
			return result;
		}

		public int GetIndexClosestAfterID(int id) => Entries.FindIndex(x => x.ID > id);

		// returns id
		public int PushMessage(string msg, ConsoleEntry.Severity lvl) => PushMessage("", msg, lvl);
		public int PushMessage(string sender, string msg, ConsoleEntry.Severity lvl)
		{
			int identifier = Entries.Count == 0 ? 0 : Entries.Last().ID + 1;

			if (Entries.Count == EntriesMax)
			{
				//TODO: OnOverflow();
				Entries.RemoveAt(0);
			}
			Entries.Add(new ConsoleEntry(sender, msg, lvl, identifier));
			OnMessage_Pushed(new MsgConsoleEventArgs(MsgConsoleEventArgs.EventType.MessagePushed, identifier, identifier));

			return identifier;
		}
		/* Methods - END */
	}
}
