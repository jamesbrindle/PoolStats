using PoolStats.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace PoolStats.Controllers
{
    public class HomeController : Controller
    {
        private PoolStatsDBEntities1 _entities = new PoolStatsDBEntities1();

        #region Properties (Cookie Control)

        protected bool Changable
        {
            get
            {
                if (CookieStore.GetCookie("changable") == null)
                    return false;
                else
                {
                    string muted = CookieStore.GetCookie("changable");

                    if (muted == "true")
                    {
                        ViewData["Changable"] = true;
                        return true;
                    }
                    else
                    {
                        ViewData["Changable"] = false;
                        return false;
                    }
                }
            }
            set
            {
                if (value)
                {
                    ViewData["Changable"] = true;
                    CookieStore.SetCookie("changable", "true", new TimeSpan(1, 0, 0, 0));
                }
                else
                {
                    ViewData["Changable"] = false;
                    CookieStore.SetCookie("changable", "false", new TimeSpan(1, 0, 0, 0));
                }
            }
        }

        protected bool MutedSound
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

        protected String PostUser
        {
            get
            {
                if (CookieStore.GetCookie("postUser") == null)
                {
                    ViewData["PostUser"] = "";
                    return "";
                }
                else
                {
                    ViewData["PostUser"] = CookieStore.GetCookie("postUser");
                    return (string)ViewData["PostUser"];
                }
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    ViewData["PostUser"] = value;
                    CookieStore.SetCookie("postUser", value, new TimeSpan(99, 0, 0, 0));
                }
                else
                {
                    ViewData["PostUser"] = value;
                    CookieStore.SetCookie("postUser", value, new TimeSpan(99, 0, 0, 0));
                }
            }
        }

        #endregion

        public ActionResult Index()
        {
            AlwaysUpdate();
            var mute = MutedSound;
            var changable = Changable;

            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }

        private void AlwaysUpdate()
        {
            HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            HttpContext.Response.Cache.SetValidUntilExpires(false);
            HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetNoStore();
        }

        #region Navigation Related

        public ActionResult Back()
        {
            AlwaysUpdate();
            var changable = Changable;
            var mute = MutedSound;

            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }

        public ActionResult BackLocked()
        {
            AlwaysUpdate();
            Changable = false;
            var mute = MutedSound;

            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }

        #endregion

        #region Sound Related

        public ViewResult Mute()
        {
            AlwaysUpdate();
            MutedSound = true;
            var changable = Changable;

            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));

        }
        public ViewResult Unmute()
        {
            AlwaysUpdate();
            MutedSound = false;
            var changable = Changable;

            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }

        #endregion

        #region Lock / Pin Related

        public ViewResult Lock()
        {
            AlwaysUpdate();
            Changable = false;
            var mute = MutedSound;

            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }

        public ViewResult Unlock()
        {
            AlwaysUpdate();
            Changable = false;
            var mute = MutedSound;

            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }

        public ActionResult EnterPin()
        {
            AlwaysUpdate();
            Changable = false;
            var mute = MutedSound;

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
                Changable = false;

                ViewData["InvalidPin"] = true;
                return View("Unlock");
            }
        }

        #endregion

        #region Player Related

        public ActionResult CreatePlayers()
        {
            AlwaysUpdate();
            var mute = MutedSound;
            var changeable = Changable;

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

        public ActionResult SubmitPlayer(Models.Player input, HttpPostedFileWrapper file, string rotateVal)
        {
            AlwaysUpdate();
            var mute = MutedSound;
            var changable = Changable;

            if (ModelState.IsValid && !String.IsNullOrEmpty(input.PlayerName))
            {
                if (_entities.Players.Any(p => p.PlayerName == input.PlayerName))
                {
                    ModelState.AddModelError("PlayerNameError", "That player name already exists.");

                    ViewData["CurrentPlayers"] = _entities.Players.ToList();
                    return View("CreatePlayers", _entities.Players.Create());
                }

                int max = _entities.Players.Max(p => p.Id);

                input.Id = max + 1;

                if (file != null)
                {
                    input.Image = input.Id + ".jpg";
                    string imagePath = Server.MapPath(Url.Content("~/Content/ProfileImages/")) + input.Image;

                    WebImage img = CropImage(new WebImage(file.InputStream), new decimal(1));

                    img = RotateImage(img, rotateVal);
                    img.Resize(300, (300 * img.Height / img.Width), true, false);

                    img.Save(imagePath, "jpg", true);
                }

                _entities.Players.Add(input);
                _entities.SaveChanges();

                return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
            }
            else
            {
                if (String.IsNullOrEmpty(input.PlayerName))
                    ModelState.AddModelError("PlayerNameError", "You have not entered a player name.");

                ViewData["CurrentPlayers"] = _entities.Players.ToList();
                return View("CreatePlayers", _entities.Players.Create());
            }
        }

        public ActionResult SubmitChangePlayer(Models.Player input, HttpPostedFileWrapper file, string rotateVal)
        {
            AlwaysUpdate();
            var mute = MutedSound;
            var changable = Changable;

            if (ModelState.IsValid && !String.IsNullOrEmpty(input.PlayerName))
            {
                Player player = _entities.Players.Find(input.Id);

                foreach (var p in _entities.Players)
                {
                    if (p.PlayerName == input.PlayerName)
                    {
                        if (p.Id != input.Id)
                        {
                            ModelState.AddModelError("PlayerNameError", "You have tried to change your name to one that already exists as a different player.");

                            ViewData["CurrentPlayers"] = _entities.Players.ToList();
                            return View("ModifyPlayer", _entities.Players.Find(input.Id));
                        }
                    }
                }

                player.PlayerName = input.PlayerName;
                player.Male = input.Male;

                if (file != null)
                {
                    input.Image = input.Id + ".jpg";
                    string imagePath = Server.MapPath(Url.Content("~/Content/ProfileImages/")) + input.Image;

                    WebImage img = CropImage(new WebImage(file.InputStream), new decimal(1));

                    img = RotateImage(img, rotateVal);
                    img.Resize(300, (300 * img.Height / img.Width), true, false);

                    img.Save(imagePath, "jpg", true);

                    player.Image = input.Image;
                }

                _entities.Entry(player).State = EntityState.Modified;
                _entities.SaveChanges();

                ViewData["CurrentPlayers"] = _entities.Players.ToList();

                return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
            }
            else
            {
                if (String.IsNullOrEmpty(input.PlayerName))
                    ModelState.AddModelError("PlayerNameError", "You have not entered a player name.");

                ViewData["CurrentPlayers"] = _entities.Players.ToList();
                return View("ModifyPlayer", _entities.Players.Find(input.Id));
            }
        }

        public ActionResult DeletePlayer(Models.Player input)
        {
            AlwaysUpdate();
            var mute = MutedSound;
            var changable = Changable;

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

            return View("CreatePlayers", _entities.Players.Create());
        }

        #endregion

        #region Two Player Match Related

        public ActionResult Create2p()
        {
            AlwaysUpdate();
            var mute = MutedSound;
            var changable = Changable;

            ViewData["CurrentPlayers"] = _entities.Players.ToList();

            return View(_entities.TwoPlayers.Create());
        }

        public ActionResult Submit2p(string player1, string player2)
        {
            AlwaysUpdate();
            var mute = MutedSound;
            var changable = Changable;

            if (player1 != player2)
            {
                List<TwoPlayer> currentTwoPlayerGames = _entities.TwoPlayers.ToList();

                foreach (var match in currentTwoPlayerGames)
                {
                    if ((match.Player1 == player1 && match.Player2 == player2) || match.Player1 == player2 && match.Player2 == player1)
                    {
                        ModelState.AddModelError(String.Empty, "A match has already been set up for the selected players.");
                        ViewData["CurrentPlayers"] = _entities.Players.ToList();

                        return View("Create2p", _entities.TwoPlayers.Create());
                    }
                }

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
            else
            {
                ModelState.AddModelError(String.Empty, "People are trying to play themselves. Although this is possible it is not in the spirit of this app and so it is not allowed.");
                ViewData["CurrentPlayers"] = _entities.Players.ToList();

                return View("Create2p", _entities.TwoPlayers.Create());
            }
        }

        public ViewResult Add2p(int id, string s, string anchorId)
        {
            AlwaysUpdate();
            var mute = MutedSound;
            var changable = Changable;

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
            var changable = Changable;

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
            var changable = Changable;

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

        #region Four Player Match Related

        public ActionResult Create4p()
        {
            AlwaysUpdate();
            var mute = MutedSound;
            var changable = Changable;

            ViewData["CurrentPlayers"] = _entities.Players.ToList();

            return View(_entities.FourPlayers.Create());
        }

        public ActionResult Submit4p(string player1, string player2, string player3, string player4)
        {
            AlwaysUpdate();
            var mute = MutedSound;
            var changable = Changable;

            if (player1 == player2)
            {
                ModelState.AddModelError(String.Empty, "Player 1 is the same as player 2");
                ViewData["CurrentPlayers"] = _entities.Players.ToList();

                return View("Create4p", _entities.FourPlayers.Create());
            }
            else if (player1 == player3)
            {
                ModelState.AddModelError(String.Empty, "Player 1 is the same as player 3");
                ViewData["CurrentPlayers"] = _entities.Players.ToList();

                return View("Create4p", _entities.FourPlayers.Create());
            }
            else if (player1 == player4)
            {
                ModelState.AddModelError(String.Empty, "Player 1 is the same as player 4");
                ViewData["CurrentPlayers"] = _entities.Players.ToList();

                return View("Create4p", _entities.FourPlayers.Create());
            }
            else if (player2 == player3)
            {
                ModelState.AddModelError(String.Empty, "Player 2 is the same as player 3");
                ViewData["CurrentPlayers"] = _entities.Players.ToList();

                return View("Create4p", _entities.FourPlayers.Create());
            }
            else if (player2 == player4)
            {
                ModelState.AddModelError(String.Empty, "Player 2 is the same as player 4");
                ViewData["CurrentPlayers"] = _entities.Players.ToList();

                return View("Create4p", _entities.FourPlayers.Create());
            }
            else if (player3 == player4)
            {
                ModelState.AddModelError(String.Empty, "Player 3 is the same as player 4");
                ViewData["CurrentPlayers"] = _entities.Players.ToList();

                return View("Create4p", _entities.FourPlayers.Create());
            }
            else
            {
                List<FourPlayer> currentFourPlayerGames = _entities.FourPlayers.ToList();

                foreach (var match in currentFourPlayerGames)
                {
                    string pl1 = match.Players1.Split(',')[0];
                    string pl2 = match.Players1.Split(',')[1];
                    string pl3 = match.Players2.Split(',')[0];
                    string pl4 = match.Players2.Split(',')[1];

                    if (pl1 == player1 && pl2 == player2 && pl3 == player3 && pl4 == player4)
                    {
                        ModelState.AddModelError(String.Empty, "A match has already been set up for the selected players.");
                        ViewData["CurrentPlayers"] = _entities.Players.ToList();

                        return View("Create4p", _entities.FourPlayers.Create());
                    }
                    else if (pl1 == player2 && pl2 == player1 && pl3 == player3 && pl4 == player4)
                    {
                        ModelState.AddModelError(String.Empty, "A match has already been set up for the selected players.");
                        ViewData["CurrentPlayers"] = _entities.Players.ToList();

                        return View("Create4p", _entities.FourPlayers.Create());
                    }
                    else if (pl1 == player2 && pl2 == player1 && pl3 == player4 && pl4 == player3)
                    {
                        ModelState.AddModelError(String.Empty, "A match has already been set up for the selected players.");
                        ViewData["CurrentPlayers"] = _entities.Players.ToList();

                        return View("Create4p", _entities.FourPlayers.Create());
                    }
                    if (pl1 == player1 && pl2 == player2 && pl3 == player4 && pl4 == player3)
                    {
                        ModelState.AddModelError(String.Empty, "A match has already been set up for the selected players.");
                        ViewData["CurrentPlayers"] = _entities.Players.ToList();

                        return View("Create4p", _entities.FourPlayers.Create());
                    }
                }

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
        }

        public ViewResult Add4p(int id, string s, string anchorId)
        {
            AlwaysUpdate();
            var mute = MutedSound;
            var changable = Changable;

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
            var changable = Changable;

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
            var changable = Changable;

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

        #region Archive View Related

        public ActionResult SoundArchive()
        {
            AlwaysUpdate();
            var mute = MutedSound;
            var changable = Changable;

            return View();
        }

        public ActionResult ImageArchive()
        {
            AlwaysUpdate();
            var mute = MutedSound;
            var changable = Changable;

            List<Post> imageList = _entities.Posts.Where(p => p.FileName != null && p.FileName != "").ToList();
            List<Post> reverseImageList = new List<Post>();

            for (int i = imageList.Count() - 1; i >= 0; i--)
            {
                reverseImageList.Add(imageList.ElementAt(i));
            }

            if (reverseImageList.Count > 200)
            {
                return View(reverseImageList.GetRange(0, 200));
            }
            else
            {
                return View(reverseImageList);
            }
        }

        public ActionResult BackupArchive()
        {

#if DEBUG
            string backupDir = "file:\\H:\\Documents\\Work\\Repositories\\PoolStats\\PoolStats\\Content\\Backups";
#else
            string backupDir = "file:\\C:\\inetpub\\wwwroot\\poolstats\\Content\\Backups";
#endif
            ViewData["BackupDir"] = backupDir;

            AlwaysUpdate();
            var mute = MutedSound;
            var changable = Changable;

            return View();
        }

        #endregion

        #region Posts Related

        public ActionResult Posts()
        {
            AlwaysUpdate();
            var mute = MutedSound;
            var changable = Changable;
            string postUser = PostUser;

            if (!String.IsNullOrEmpty(PostUser))
                ViewData["PlayerName"] = GetPlayerNameFromID(Convert.ToInt32(postUser));

            List<Post> reverseList = new List<Post>();

            for (int i = _entities.Posts.Count() - 1; i >= 0; i--)
            {
                reverseList.Add(_entities.Posts.ToList().ElementAt(i));
            }

            if (reverseList.Count > 200)
            {
                return View("Posts", Tuple.Create(_entities.Players.ToList(), reverseList.GetRange(0, 200)));
            }
            else
            {
                return View("Posts", Tuple.Create(_entities.Players.ToList(), reverseList));
            }
        }

        public ActionResult SubmitPost(string message, string playerId, HttpPostedFileWrapper file, string rotateVal)
        {
            AlwaysUpdate();
            var mute = MutedSound;
            var changable = Changable;
            PostUser = playerId;

            if (!String.IsNullOrEmpty(PostUser))
                ViewData["PlayerName"] = GetPlayerNameFromID(Convert.ToInt32(playerId));

            if (string.IsNullOrEmpty(message) && file == null)
            {
                return View("Posts", Tuple.Create(_entities.Players.ToList(), _entities.Posts.ToList()));
            }
            else
            {
                Post post = new Post();

                post.PlayerName = GetPlayerNameFromID(Convert.ToInt32(playerId));
                post.Message = String.IsNullOrEmpty(message) ? "" : message;
                post.PostDate = DateTime.Now;

                int id = 0;

                try
                {
                    id = _entities.Posts.Max(p => p.ID) + 1;
                }
                catch { }

                post.ID = id;

                if (file != null)
                {
                    post.FileName = id + ".jpg";
                    string imagePath = Server.MapPath(Url.Content("~/Content/Posts/Images/")) + post.FileName;

                    WebImage img = new WebImage(file.InputStream);

                    img = RotateImage(img, rotateVal);
                    img.Resize(600, (600 * img.Height / img.Width), true, false);

                    img.Save(imagePath, "jpg", true);
                }

                _entities.Posts.Add(post);
                _entities.Entry(post).State = EntityState.Added;
                _entities.SaveChanges();

                List<Post> reverseList = new List<Post>();

                for (int i = _entities.Posts.Count() - 1; i >= 0; i--)
                {
                    reverseList.Add(_entities.Posts.ToList().ElementAt(i));
                }

                if (reverseList.Count > 200)
                {
                    return View("Posts", Tuple.Create(_entities.Players.ToList(), reverseList.GetRange(0, 200)));
                }
                else
                {
                    return View("Posts", Tuple.Create(_entities.Players.ToList(), reverseList));
                }
            }
        }

        public ActionResult DeletePost(int id, int playerId)
        {
            AlwaysUpdate();
            var mute = MutedSound;
            var changable = Changable;

            if (!String.IsNullOrEmpty(PostUser))
                ViewData["PlayerName"] = GetPlayerNameFromID(Convert.ToInt32(playerId));

            try
            {
                Post post = _entities.Posts.Single(p => p.ID == id);
                _entities.Entry(post).State = EntityState.Deleted;

                _entities.SaveChanges();
            }
            catch { }


            List<Post> reverseList = new List<Post>();

            for (int i = _entities.Posts.Count() - 1; i >= 0; i--)
            {
                reverseList.Add(_entities.Posts.ToList().ElementAt(i));
            }

            if (reverseList.Count > 200)
            {
                return View("Posts", Tuple.Create(_entities.Players.ToList(), reverseList.GetRange(0, 200)));
            }
            else
            {
                return View("Posts", Tuple.Create(_entities.Players.ToList(), reverseList));
            }
        }

        #endregion

        #region Misc

        public string GetPlayerNameFromID(int id)
        {
            return _entities.Players.SingleOrDefault(p => p.Id == id).PlayerName;
        }

        #endregion

        #region Image Processing

        public static Image ResizeImage(Image image, int maxWidth = 0, int maxHeight = 0)
        {
            if (maxWidth == 0)
                maxWidth = image.Width;
            if (maxHeight == 0)
                maxHeight = image.Height;

            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);
            return newImage;
        }

        public static WebImage CropImage(WebImage image, decimal targetRatio)
        {
            decimal currentImageRatio = image.Width / (decimal)image.Height;
            int difference;

            //image is wider than targeted
            if (currentImageRatio > targetRatio)
            {
                int targetWidth = Convert.ToInt32(Math.Floor(targetRatio * image.Height));
                difference = image.Width - targetWidth;
                int left = Convert.ToInt32(Math.Floor(difference / (decimal)2));
                int right = Convert.ToInt32(Math.Ceiling(difference / (decimal)2));
                image.Crop(0, left, 0, right);
            }
            //image is higher than targeted
            else if (currentImageRatio < targetRatio)
            {
                int targetHeight = Convert.ToInt32(Math.Floor(image.Width / targetRatio));
                difference = image.Height - targetHeight;
                int top = Convert.ToInt32(Math.Floor(difference / (decimal)2));
                int bottom = Convert.ToInt32(Math.Ceiling(difference / (decimal)2));
                image.Crop(top, 0, bottom, 0);
            }
            return image;
        }

        public static WebImage RotateImage(WebImage image, string rotateVal)
        {
            if (rotateVal != "0")
            {
                if (rotateVal == "90")
                {
                    image.RotateRight();
                }
                else if (rotateVal == "180")
                {
                    image.RotateRight();
                    image.RotateRight();
                }
                else if (rotateVal == "270")
                {
                    image.RotateRight();
                    image.RotateRight();
                    image.RotateRight();
                }
                else if (rotateVal == "-90")
                {
                    image.RotateLeft();
                }
                else if (rotateVal == "-180")
                {
                    image.RotateLeft();
                    image.RotateLeft();
                }
                else if (rotateVal == "-270")
                {
                    image.RotateLeft();
                    image.RotateLeft();
                    image.RotateLeft();
                }
            }

            return image;
        }

        #endregion
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