using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupTask
{
    class Program
    {
#if DEBUG
      private static string outputDir = "..\\..\\..\\PoolStats\\Content\\Backups\\";
#else
      private static string outputDir = "..\\..\\Content\\Backups\\";
#endif
        private static List<TwoPlayerScore> _twoPlayerScores = new List<TwoPlayerScore>();
        private static List<FourPlayerScore> _fourPlayerScores = new List<FourPlayerScore>();
        private static List<TwoPlayerStat> _twoPlayerStats = new List<TwoPlayerStat>();

        static void Main(string[] args)
        {
            if (!HasBackedUpToday())
            {
                GenerateMatchLists();
                WorkOutPercentages();
                OutputToFile();
            }
        }

        private static bool HasBackedUpToday()
        {
            DirectoryInfo d = new DirectoryInfo(outputDir);
            FileInfo[] Files = d.GetFiles("*.txt"); //Getting Text files

            string todaysDate = DateTime.Now.ToLongDateString();

            foreach (FileInfo file in Files)
            {
                if (file.Name.Contains(todaysDate))
                    return true;
            }

            return false;
        }

        private static void OutputToFile()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(string.Format("--- Pool Stats Backup [{0}] ---\n\r\n\r", DateTime.Now.ToString()));

            sb.Append("- Player Stats -\n\r\n\r\n\r");

            foreach (var stat in _twoPlayerStats)
            {
                sb.Append(string.Format("Player: {0}\n\r", stat.PlayerName));
                sb.Append(string.Format("Total Games: {0}\n\r", stat.TotalGames));
                sb.Append(string.Format("Total Games Won: {0}\n\r", stat.TotalGamesWon));
                sb.Append(string.Format("Total Games Lost: {0}\n\r", stat.TotalGamesLost));
                sb.Append(string.Format("Total Points: {0}\n\r", stat.Points));
                sb.Append(string.Format("Percentage Winning: {0}%\n\r", Double.IsNaN(stat.PercentageWinning) ? 0.0 : stat.PercentageWinning));
                sb.Append(string.Format("Player Commitment: {0}%\n\r\n\r", Double.IsNaN(stat.PlayerCommitment) ? 0.0 : stat.PlayerCommitment));
            }

            sb.Append("\n- Two Player Matches -\n\r\n\r\n\r");

            foreach (var match in _twoPlayerScores)
            {
                sb.Append(string.Format("Player 1: {0} Score: {1} ({2}%)\n\r",
                     match.Player1_Name, match.Score1, Double.IsNaN(match.PercentageWinning1) ? 0.0 : match.PercentageWinning1));

                sb.Append(string.Format("Player 2: {0} Score: {1} ({2}%)\n\r\n\r",
                     match.Player2_Name, match.Score2, Double.IsNaN(match.PercentageWinning2) ? 0.0 : match.PercentageWinning2));
            }

            sb.Append("\n- Four Player Matches -\n\r\n\r\n\r");

            foreach (var match in _fourPlayerScores)
            {
                sb.Append(string.Format("Players 1: {0} Score: {1} ({2}%)\n\r",
                    match.Players1_Name, match.Score1, Double.IsNaN(match.PercentageWinning1) ? 0.0 : match.PercentageWinning1));

                sb.Append(string.Format("Players 2: {0} Score: {1} ({2}%)\n\r",
                    match.Players2_Name, match.Score2, Double.IsNaN(match.PercentageWinning2) ? 0.0 : match.PercentageWinning2));
            }

            sb.Append("\n\r");

            FileStream fs1 = new FileStream(outputDir + String.Format("Backup {0}.txt", DateTime.Now.ToLongDateString()), FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fs1);
            writer.Write(sb.ToString());
            writer.Close();
        }

        private static void GenerateMatchLists()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {

#if DEBUG
                conn.ConnectionString =
                    "Data Source=(LocalDb)\\MSSQLLocalDB;" +
                    "Initial Catalog=PoolStatsDB;" +
                    "Integrated Security=SSPI;";
#else
                    conn.ConnectionString =
                        "Data Source=.\\SQLEXPRESS;" +
                        "Initial Catalog=PoolStatsDB;" +
                        //"User Id = jb-net-admin1;Password = hellothere;" +
                        "Integrated Security=SSPI;" +
                        "AttachDbFilename = C:\\inetpub\\wwwroot\\poolstats\\App_Data\\PoolStatsDB.mdf; ";
#endif

                    conn.Open();

                    SqlCommand command = new SqlCommand(
                        @"SELECT p1.Id [PlayerID],
		                     p1.PlayerName,
		                     tp.Score1,
		                     p2.Id [PlayerID],
		                     p2.PlayerName,
		                     tp.Score2
                     FROM dbo.TwoPlayer tp
	                    INNER JOIN dbo.Players p1
		                    on p1.Id = tp.Player1
	                    INNER JOIN dbo.Players p2
		                    on p2.Id = tp.Player2"
                        , conn);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // while there is another record present
                        while (reader.Read())
                        {
                            var twoPlayerScore = new TwoPlayerScore();

                            twoPlayerScore.Player1_ID = Convert.ToInt32(reader[0]);
                            twoPlayerScore.Player1_Name = Convert.ToString(reader[1]);
                            twoPlayerScore.Score1 = Convert.ToInt32(reader[2]);

                            twoPlayerScore.Player2_ID = Convert.ToInt32(reader[3]);
                            twoPlayerScore.Player2_Name = Convert.ToString(reader[4]);
                            twoPlayerScore.Score2 = Convert.ToInt32(reader[5]);

                            _twoPlayerScores.Add(twoPlayerScore);
                        }
                    }

                    command = new SqlCommand(
                        @"SELECT	fp.Players1 [PlayerIds],
		                    p1a.PlayerName + ' & ' + p1b.PlayerName [PlayerNames],
		                    fp.Score1,
                            fp.Players2 [PlayerIds],
		                    p2a.PlayerName + ' & ' + p2b.PlayerName [PlayerNames],
		                    Score2
                     FROM dbo.FourPlayer fp
	                    INNER JOIN dbo.Players p1a
		                    on p1a.Id = (
				                    SELECT result.val
				                    FROM (
					                    SELECT [Value] as val, ROW_NUMBER() OVER (ORDER BY [Index]) AS rowNum 
					                    FROM dbo.SplitString(fp.Players1, ',')
					                    ) result
				                    WHERE result.rowNum = 1)
	                    INNER JOIN dbo.Players p1b
		                    on p1b.Id = (
				                    SELECT result.val
				                    FROM (
					                    SELECT [Value] as val, ROW_NUMBER() OVER (ORDER BY [Index]) AS rowNum 
					                    FROM dbo.SplitString(fp.Players1, ',')
					                    ) result
				                    WHERE result.rowNum = 2)
	                    INNER JOIN dbo.Players p2a
		                    on p2a.Id = (
				                    SELECT result.val
				                    FROM (
					                    SELECT [Value] as val, ROW_NUMBER() OVER (ORDER BY [Index]) AS rowNum 
					                    FROM dbo.SplitString(fp.Players2, ',')
					                    ) result
				                    WHERE result.rowNum = 1)
	                    INNER JOIN dbo.Players p2b
		                    on p2b.Id = (
				                    SELECT result.val
				                    FROM (
					                    SELECT [Value] as val, ROW_NUMBER() OVER (ORDER BY [Index]) AS rowNum 
					                    FROM dbo.SplitString(fp.Players2, ',')
					                    ) result
				                    WHERE result.rowNum = 2)"
                        , conn);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // while there is another record present
                        while (reader.Read())
                        {
                            var fourPlayerScore = new FourPlayerScore();

                            fourPlayerScore.Players1_IDs = Convert.ToString(reader[0]);
                            fourPlayerScore.Players1_Name = Convert.ToString(reader[1]);
                            fourPlayerScore.Score1 = Convert.ToInt32(reader[2]);

                            fourPlayerScore.Players2_IDs = Convert.ToString(reader[3]);
                            fourPlayerScore.Players2_Name = Convert.ToString(reader[4]);
                            fourPlayerScore.Score2 = Convert.ToInt32(reader[5]);

                            _fourPlayerScores.Add(fourPlayerScore);
                        }
                    }

                    conn.Close();

                }

            }
            catch(Exception e)
            {
                FileStream fs1 = new FileStream(outputDir + "\\ErrorLog\\" + String.Format("ErrorLog {0}.txt", DateTime.Now.ToLongDateString()), FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter writer = new StreamWriter(fs1);
                writer.Write(e.Message);
                writer.Close();
            }
        }

        private static void WorkOutPercentages()
        {
            int totalAllGames = 0;

            // Player 1

            foreach (TwoPlayerScore match in _twoPlayerScores)
            {
                bool hasPlayer = false;

                foreach (TwoPlayerStat stat in _twoPlayerStats)
                {
                    if (stat.PlayerID == match.Player1_ID)
                    {
                        hasPlayer = true;

                        stat.TotalGamesWon = stat.TotalGamesWon + match.Score1;
                        stat.TotalGamesLost = stat.TotalGamesLost + match.Score2;
                        stat.TotalGames = stat.TotalGamesWon + stat.TotalGamesLost;
                    }
                }

                if (!hasPlayer)
                {
                    TwoPlayerStat newStat = new TwoPlayerStat();
                    newStat.PlayerID = match.Player1_ID;
                    newStat.PlayerName = match.Player1_Name;
                    newStat.TotalGamesWon = match.Score1;
                    newStat.TotalGamesLost = match.Score2;
                    newStat.TotalGames = match.Score1 + match.Score2;
                    newStat.PercentageWinning = 0.0;
                    newStat.PlayerCommitment = 0.0;

                    _twoPlayerStats.Add(newStat);
                }
            }

            // Player 2

            foreach (TwoPlayerScore match in _twoPlayerScores)
            {
                bool hasPlayer = false;

                foreach (TwoPlayerStat stat in _twoPlayerStats)
                {
                    if (stat.PlayerID == match.Player2_ID)
                    {
                        hasPlayer = true;

                        stat.TotalGamesWon = stat.TotalGamesWon + match.Score2;
                        stat.TotalGamesLost = stat.TotalGamesLost + match.Score1;
                        stat.TotalGames = stat.TotalGamesWon + stat.TotalGamesLost;
                    }
                }

                if (!hasPlayer)
                {
                    TwoPlayerStat newStat = new TwoPlayerStat();
                    newStat.PlayerID = match.Player2_ID;
                    newStat.PlayerName = match.Player2_Name;
                    newStat.TotalGamesWon = match.Score2;
                    newStat.TotalGamesLost = match.Score1;
                    newStat.TotalGames = match.Score1 + match.Score2;
                    newStat.PercentageWinning = 0.0;
                    newStat.PlayerCommitment = 0.0;

                    _twoPlayerStats.Add(newStat);
                }
            }

            // Percentages

            foreach (TwoPlayerStat stat in _twoPlayerStats)
            {
                totalAllGames = totalAllGames + stat.TotalGames;
            }

            foreach (TwoPlayerStat stat in _twoPlayerStats)
            {
                stat.PercentageWinning = Math.Round((double)stat.TotalGamesWon / (double)stat.TotalGames * (double)100);
            }

            foreach (TwoPlayerStat stat in _twoPlayerStats)
            {
                stat.PlayerCommitment = Math.Round((double)stat.TotalGames / (double)totalAllGames * (double)100, 2);
            }

            foreach (TwoPlayerStat stat in _twoPlayerStats)
            {
                stat.Points = (double)stat.TotalGamesWon + ((double)stat.TotalGamesLost * (double)0.5);
            }

            foreach (TwoPlayerScore match in _twoPlayerScores)
            {
                try
                {
                    match.PercentageWinning1 = Math.Round((double)match.Score1 / (double)(match.Score1 + match.Score2) * (double)100, 2);
                }
                catch { }

                try
                {
                    match.PercentageWinning2 = Math.Round((double)match.Score2 / (double)(match.Score1 + match.Score2) * (double)100, 2);
                }
                catch { }
            }

            foreach (FourPlayerScore match in _fourPlayerScores)
            {
                try
                {
                    match.PercentageWinning1 = Math.Round((double)match.Score1 / (double)(match.Score1 + match.Score2) * (double)100, 2);
                }
                catch { }

                try
                {
                    match.PercentageWinning2 = Math.Round((double)match.Score2 / (double)(match.Score1 + match.Score2) * (double)100, 2);
                }
                catch { }
            }
        }
    }
}
