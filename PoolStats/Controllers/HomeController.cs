using PoolStats.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PoolStats.Controllers
{
    public class HomeController : Controller
    {
        private PoolStatsDB_Entities _entities = new PoolStatsDB_Entities();

        public bool Changable
        {
            get
            {
                if (ViewData["Changeable"] == null)
                {
                    ViewData["Changable"] = false;
                }

                return (bool)ViewData["Changable"];
            }
            set
            {
                ViewData["Changable"] = value;
            }
        }

        public bool MutedSound
        {
            get
            {
                if (CookieStore.GetCookie("muted") == null)
                    return false;
                else
                {
                    string muted = CookieStore.GetCookie("muted");

                    if (muted == "true")
                    {
                        ViewData["MutedSound"] = true;
                        return true;
                    }
                    else
                    {
                        ViewData["MutedSound"] = false;
                        return false;
                    }
                }
            }
            set
            {
                if (value)
                {
                    ViewData["MutedSound"] = true;
                    CookieStore.SetCookie("muted", "true", new TimeSpan(2, 0, 0, 0));
                }
                else
                {
                    ViewData["MutedSound"] = false;
                    CookieStore.SetCookie("muted", "false", new TimeSpan(2, 0, 0, 0));
                }
            }
        }

        private void AlwaysUpdate()
        {
            HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            HttpContext.Response.Cache.SetValidUntilExpires(false);
            HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetNoStore();
        }

        public ActionResult Index()
        {
            AlwaysUpdate();
            var mute = MutedSound;

            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }

        public ActionResult Back()
        {
            Changable = true;
            var mute = MutedSound;
            AlwaysUpdate();

            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }

        public ActionResult BackLocked()
        {
            Changable = false;
            var mute = MutedSound;
            AlwaysUpdate();

            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }


        public ViewResult Unlock()
        {
            Changable = true;
            var mute = MutedSound;
            AlwaysUpdate();

            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }

        public ViewResult Mute(bool changeable)
        {
            MutedSound = true;
            Changable = changeable;

            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));

        }
        public ViewResult Unmute(bool changeable)
        {
            MutedSound = false;
            Changable = changeable;

            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }

        public ViewResult Lock()
        {

            Changable = false;
            var mute = MutedSound;
            AlwaysUpdate();

            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }

        public ActionResult EnterPin()
        {
            Changable = true;
            var mute = MutedSound;
            AlwaysUpdate();

            return View("Unlock");
        }

        public ActionResult ValidatePin(string ePin)
        {
            AlwaysUpdate();
            var mute = MutedSound;

            string correctPin = _entities.Pins.FirstOrDefault().PinNumber;

            if (ePin == correctPin)
            {
                Changable = true;

                ViewData["InvalidPin"] = false;
                return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
            }
            else
            {
                ViewData["InvalidPin"] = true;
                return View("Unlock");
            }
        }

        public ActionResult CreatePlayers()
        {
            AlwaysUpdate();
            var mute = MutedSound;
            Changable = true;

            ViewData["CurrentPlayers"] = _entities.Players.ToList();

            return View(_entities.Players.Create());
        }

        public ActionResult ModifyPlayer(int id)
        {
            AlwaysUpdate();
            var mute = MutedSound;
            Changable = true;

            return View(_entities.Players.Find(id));
        }


        public ActionResult SubmitPlayer(Models.Player input)
        {
            AlwaysUpdate();
            var mute = MutedSound;
            Changable = true;

            int max = _entities.Players.Max(p => p.Id);

            input.Id = max + 1;

            _entities.Players.Add(input);
            _entities.SaveChanges();

            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }

        public ActionResult SubmitChangePlayer(Models.Player input)
        {
            AlwaysUpdate();
            var mute = MutedSound;
            Changable = true;

            Player player = _entities.Players.Find(input.Id);
            player.PlayerName = input.PlayerName;
            _entities.Entry(player).State = EntityState.Modified;

            _entities.SaveChanges();

            ViewData["CurrentPlayers"] = _entities.Players.ToList();

            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }

        public ActionResult DeletePlayer(Models.Player input)
        {
            AlwaysUpdate();
            var mute = MutedSound;
            Changable = true;

            Player player = _entities.Players.Find(input.Id);

            bool exists = false;

            foreach (TwoPlayer item in _entities.TwoPlayers)
            {
                try
                {
                    if (Convert.ToInt32(item.Player1) == input.Id)
                    {
                        exists = true;
                    }
                    if (Convert.ToInt32(item.Player2) == input.Id)
                    {
                        exists = true;
                    }
                }
                catch { }
            }
            foreach (FourPlayer item in _entities.FourPlayers)
            {
                try
                {
                    string[] parts = item.Players1.Split(',');

                    if (Convert.ToInt32(parts[0]) == input.Id)
                    {
                        exists = true;
                    }
                    if (Convert.ToInt32(parts[1]) == input.Id)
                    {
                        exists = true;
                    }

                    parts = item.Players2.Split(',');

                    if (Convert.ToInt32(parts[0]) == input.Id)
                    {
                        exists = true;
                    }
                    if (Convert.ToInt32(parts[1]) == input.Id)
                    {
                        exists = true;
                    }
                }
                catch { }
            }

            if (!exists)
            {
                _entities.Entry(player).State = EntityState.Deleted;
                _entities.SaveChanges();
            }

            ViewData["CurrentPlayers"] = _entities.Players.ToList();

            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }


        #region Two Player

        public ActionResult Create2p()
        {
            AlwaysUpdate();
            var mute = MutedSound;
            Changable = true;

            ViewData["CurrentPlayers"] = _entities.Players.ToList();

            return View(_entities.TwoPlayers.Create());
        }

        public ActionResult Submit2p(string player1, string player2)
        {
            AlwaysUpdate();
            var mute = MutedSound;
            Changable = true;

            var twoPlayerModel = new PoolStats.Models.TwoPlayer();

            twoPlayerModel.Player1 = player1;
            twoPlayerModel.Player2 = player2;

            twoPlayerModel.Score1 = 0;
            twoPlayerModel.Score2 = 0;

            int max = 0;

            try
            {
                max = _entities.TwoPlayers.Max(p => p.ID);
            }
            catch { }

            twoPlayerModel.ID = max + 1;

            _entities.TwoPlayers.Add(twoPlayerModel);
            _entities.SaveChanges();

            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }

        public ViewResult Add2p(int id, string s, string anchorId)
        {
            AlwaysUpdate();
            var mute = MutedSound;
            Changable = true;

            ViewData["PlaySound"] = true;
            ViewData["Sound"] = "cheer";

            TwoPlayer score = _entities.TwoPlayers.Where(p => p.ID == id).FirstOrDefault();

            if (s == "1")
                score.Score1 = score.Score1 + 1;
            else if (s == "2")
                score.Score2 = score.Score2 + 1;

            _entities.SaveChanges();

            ViewData["AnchorID"] = anchorId;
            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }

        public ViewResult Subtract2p(int id, string s, string anchorId)
        {
            AlwaysUpdate();
            var mute = MutedSound;
            Changable = true;

            ViewData["PlaySound"] = true;
            ViewData["Sound"] = "boo";

            TwoPlayer score = _entities.TwoPlayers.Where(p => p.ID == id).FirstOrDefault();

            if (s == "1")
                score.Score1 = score.Score1 - 1;
            else if (s == "2")
                score.Score2 = score.Score2 - 1;

            _entities.SaveChanges();

            ViewData["AnchorID"] = anchorId;
            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }

        public ViewResult Delete2p(int id, string anchorId)
        {
            AlwaysUpdate();
            var mute = MutedSound;
            Changable = true;

            TwoPlayer score = _entities.TwoPlayers.Where(p => p.ID == id).FirstOrDefault();

            try
            {
                _entities.TwoPlayers.Remove(score);
            }
            catch { }

            _entities.SaveChanges();

            ViewData["AnchorID"] = anchorId;
            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }

        #endregion

        #region Four Player

        public ActionResult Create4p()
        {
            AlwaysUpdate();
            var mute = MutedSound;
            Changable = true;

            ViewData["CurrentPlayers"] = _entities.Players.ToList();

            return View(_entities.FourPlayers.Create());
        }

        public ActionResult Submit4p(string player1, string player2, string player3, string player4)
        {
            AlwaysUpdate();
            var mute = MutedSound;
            Changable = true;

            var fourPlayerModel = new PoolStats.Models.FourPlayer();

            fourPlayerModel.Players1 = player1 + "," + player2;
            fourPlayerModel.Players2 = player3 + "," + player4;

            fourPlayerModel.Score1 = 0;
            fourPlayerModel.Score2 = 0;

            int max = 0;

            try
            {
                max = _entities.FourPlayers.Max(p => p.ID);
            }
            catch { }

            fourPlayerModel.ID = max + 1;

            _entities.FourPlayers.Add(fourPlayerModel);
            _entities.SaveChanges();

            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }

        public ViewResult Add4p(int id, string s, string anchorId)
        {
            AlwaysUpdate();
            var mute = MutedSound;
            Changable = true;

            ViewData["PlaySound"] = true;
            ViewData["Sound"] = "cheer";

            FourPlayer score = _entities.FourPlayers.Where(p => p.ID == id).FirstOrDefault();

            if (s == "1")
                score.Score1 = score.Score1 + 1;
            else if (s == "2")
                score.Score2 = score.Score2 + 1;

            _entities.SaveChanges();

            ViewData["AnchorID"] = anchorId;
            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }

        public ViewResult Subtract4p(int id, string s, string anchorId)
        {
            AlwaysUpdate();
            var mute = MutedSound;
            Changable = true;

            ViewData["PlaySound"] = true;
            ViewData["Sound"] = "boo";

            FourPlayer score = _entities.FourPlayers.Where(p => p.ID == id).FirstOrDefault();

            if (s == "1")
                score.Score1 = score.Score1 - 1;
            else if (s == "2")
                score.Score2 = score.Score2 - 1;

            _entities.SaveChanges();

            ViewData["AnchorID"] = anchorId;
            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }

        public ViewResult Delete4p(int id, string anchorId)
        {
            AlwaysUpdate();
            var mute = MutedSound;
            Changable = true;

            FourPlayer score = _entities.FourPlayers.Where(p => p.ID == id).FirstOrDefault();

            try
            {
                _entities.FourPlayers.Remove(score);
            }
            catch { }

            _entities.SaveChanges();

            ViewData["AnchorID"] = anchorId;
            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }

        #endregion

        public ActionResult SoundArchive(string locked)
        {
            AlwaysUpdate();
            var mute = MutedSound;
            Changable = Convert.ToBoolean(locked);

            return View();
        }

        public ActionResult ImageArchive(string locked)
        {
            AlwaysUpdate();
            var mute = MutedSound;
            Changable = Convert.ToBoolean(locked);

            return View();
        }

        public ActionResult BackupArchive(string locked)
        {
            AlwaysUpdate();
            var mute = MutedSound;
            Changable = Convert.ToBoolean(locked);

            return View();
        }

    }

    public class CookieStore
    {
        public static void SetCookie(string key, string value, TimeSpan expires)
        {
            HttpCookie cookie = new HttpCookie(key, value);

            if (HttpContext.Current.Request.Cookies[key] != null)
            {
                var cookieOld = HttpContext.Current.Request.Cookies[key];
                cookieOld.Expires = DateTime.Now.Add(expires);
                cookieOld.Value = cookie.Value;
                HttpContext.Current.Response.Cookies.Add(cookieOld);
            }
            else
            {
                cookie.Expires = DateTime.Now.Add(expires);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
        public static string GetCookie(string key)
        {
            string value = string.Empty;
            HttpCookie cookie = HttpContext.Current.Request.Cookies[key];

            if (cookie != null)
            {
                value = cookie.Value;
            }
            return value;
        }
    }
}