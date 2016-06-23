using PoolStats.Models;
using System;
using System.Collections.Generic;
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

        public ActionResult Index()
        {
            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }

        public ActionResult Back()
        {
            Changable = true;

            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }

        public ActionResult BackLocked()
        {
            Changable = false;

            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }


        public ViewResult Unlock()
        {

            Changable = true;

            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }
        public ViewResult Lock()
        {

            Changable = false;

            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }

        public ActionResult EnterPin()
        {
            Changable = true;

            return View("Unlock");
        }

        public ActionResult ValidatePin(string ePin)
        {
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
            Changable = true;

            ViewData["CurrentPlayers"] = _entities.Players.ToList();

            return View(_entities.Players.Create());
        }


        public ActionResult SubmitPlayer(Models.Player input)
        {
            Changable = true;

            int max = _entities.Players.Max(p => p.Id);

            input.Id = max + 1;

            _entities.Players.Add(input);
            _entities.SaveChanges();

            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }


        #region Two Player

        public ActionResult Create2p()
        {
            Changable = true;

            ViewData["CurrentPlayers"] = _entities.Players.ToList();

            return View(_entities.TwoPlayers.Create());
        }

        public ActionResult Submit2p(string player1, string player2)
        {
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

        public ViewResult Add2p(int id, string s)
        {
            Changable = true;

            TwoPlayer score = _entities.TwoPlayers.Where(p => p.ID == id).FirstOrDefault();

            if (s == "1")
                score.Score1 = score.Score1 + 1;
            else if (s == "2")
                score.Score2 = score.Score2 + 1;

            _entities.SaveChanges();

            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }

        public ViewResult Subtract2p(int id, string s)
        {
            Changable = true;

            TwoPlayer score = _entities.TwoPlayers.Where(p => p.ID == id).FirstOrDefault();

            if (s == "1")
                score.Score1 = score.Score1 - 1;
            else if (s == "2")
                score.Score2 = score.Score2 - 1;

            _entities.SaveChanges();

            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }

        public ViewResult Delete2p(int id)
        {
            Changable = true;

            TwoPlayer score = _entities.TwoPlayers.Where(p => p.ID == id).FirstOrDefault();

            _entities.TwoPlayers.Remove(score);

            _entities.SaveChanges();

            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }

        #endregion

        #region Four Player

        public ActionResult Create4p()
        {
            Changable = true;

            ViewData["CurrentPlayers"] = _entities.Players.ToList();

            return View(_entities.FourPlayers.Create());
        }

        public ActionResult Submit4p(string player1, string player2, string player3, string player4)
        {
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

        public ViewResult Add4p(int id, string s)
        {
            Changable = true;

            FourPlayer score = _entities.FourPlayers.Where(p => p.ID == id).FirstOrDefault();

            if (s == "1")
                score.Score1 = score.Score1 + 1;
            else if (s == "2")
                score.Score2 = score.Score2 + 1;

            _entities.SaveChanges();

            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }

        public ViewResult Subtract4p(int id, string s)
        {
            Changable = true;

            FourPlayer score = _entities.FourPlayers.Where(p => p.ID == id).FirstOrDefault();

            if (s == "1")
                score.Score1 = score.Score1 - 1;
            else if (s == "2")
                score.Score2 = score.Score2 - 1;

            _entities.SaveChanges();

            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }

        public ViewResult Delete4p(int id)
        {
            Changable = true;

            FourPlayer score = _entities.FourPlayers.Where(p => p.ID == id).FirstOrDefault();

            _entities.FourPlayers.Remove(score);

            _entities.SaveChanges();

            return View("Index", Tuple.Create(_entities.TwoPlayers.ToList(), _entities.FourPlayers.ToList(), _entities.Players.ToList()));
        }

        #endregion

    }
}