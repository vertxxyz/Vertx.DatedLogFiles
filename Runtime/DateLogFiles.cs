using System;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace Vertx.DatedLogFiles
{
	public class DateLogFiles : MonoBehaviour
	{
		private const string LoggedKey = "dated-log-files--date: ";
		private const string Format = "yyyy-MM-dd HH-mm-ss";
		
#pragma warning disable CS0219,CS0168
		// ReSharper disable once NotAccessedField.Local
		[Tooltip("Logs from development builds are renamed by default.")]
		[SerializeField] private bool _renameInReleaseBuilds;
#pragma warning restore CS0219,CS0168
		[Tooltip("Logs will not be removed if this value is <= 0.")]
		[SerializeField] private int _removeLogsOlderThanDays = 30;


#if !UNITY_EDITOR && UNITY_STANDALONE
		private void Awake()
		{
#if DEBUG
			DoRun();
#else
			if (_renameInReleaseBuilds)
				DoRun();
#endif
		}
#endif

		/// <summary>
		/// Adds a formatted date to the current log file, 
		/// removes log files older than x days, 
		/// and renames the previous log file with its date if listed.
		/// </summary>
		/// <param name="removeLogsOlderThanDays">Logs will not be removed if this value is less than or equal to 0.</param>
		// ReSharper disable once MemberCanBePrivate.Global
		public static void Run(int removeLogsOlderThanDays = 30)
		{
			DateTime now = DateTime.Now;
			AddDateToCurrentLogFile(now);
			RemoveOldLogFiles(now, removeLogsOlderThanDays);
			RenamePrevLogFile();
		}

		private void DoRun() => Run(_removeLogsOlderThanDays);

		private static async void RenamePrevLogFile()
		{
			try
			{
				string prevLogPath = Path.Combine(Application.persistentDataPath, "Player-prev.log");
				Console.WriteLine(prevLogPath);
				if (!File.Exists(prevLogPath))
					return;

				string date;
				using (FileStream fileStream = File.OpenRead(prevLogPath))
				using (StreamReader reader = new StreamReader(fileStream))
				{
					while (true)
					{
						string line = await reader.ReadLineAsync();
						if (line == null)
							return;
						if (!line.StartsWith(LoggedKey, StringComparison.Ordinal))
							continue;
						date = line.Substring(LoggedKey.Length);
						break;
					}
				}

				// ReSharper disable once AssignNullToNotNullAttribute
				string fileName = Path.Combine(Path.GetDirectoryName(prevLogPath), $"Player-{date}.log");
				File.Move(prevLogPath, fileName);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		private static void RemoveOldLogFiles(DateTime now, int removeLogsOlderThanDays)
		{
			if (removeLogsOlderThanDays <= 0)
				return;

			foreach (string path in Directory.EnumerateFiles(Application.persistentDataPath, "Player-????-??-?? ??-??-??.log"))
			{
				try
				{
					string date = path.Substring(path.Length - (Format.Length + ".log".Length), Format.Length);
					if (!DateTime.TryParseExact(date, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime logDateTime))
						continue;
					TimeSpan span = now - logDateTime;
					if (span.TotalDays > removeLogsOlderThanDays)
						File.Delete(path);
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
			}
		}

		private static void AddDateToCurrentLogFile(DateTime dateTime)
			=> Console.WriteLine($"{LoggedKey}{dateTime.ToString(Format, CultureInfo.InvariantCulture)}");
	}
}