using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PI3.Database;
using System.Web.Security;
using PI3.Database;

namespace PI3.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            using (var db = new alphasupermarketEntities())
            {
                int quantidadeParaMostrar = 9;

                // retorna produtos ativos, ordenados randomicamente pegado so os quantidadeParaMostrar primeiros
                var produtosAleatorios = db.Produto.Where(p => p.ativoProduto == "1").OrderBy(p => Guid.NewGuid()).Take(quantidadeParaMostrar).ToList();

                // os 4 produtos mais vendidos
                var ids4MaisVendidos = db.ItemPedido.Include("Produto").GroupBy(p => p.idProduto).Select(gr => new { id = gr.Key, count = gr.Count() }).OrderByDescending(p => p.count).Take(4).Select(g => g.id).ToList();
                ViewBag.maisVendidos = db.Produto.Where(p => ids4MaisVendidos.Contains(p.idProduto)).ToList();

                return View(produtosAleatorios);
            }
        }

        public ActionResult Pesquisa(string categoria, string termo)
        {
            using (var db = new alphasupermarketEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                List<Produto> lista = new List<Produto>();

                if (!String.IsNullOrEmpty(categoria))
                {
                    if (categoria == "todos")
                    {
                        lista = db.Produto.Where(p => p.ativoProduto == "1").OrderBy(p => p.nomeProduto).ToList();
                    }
                    else
                    {
                        int id = int.Parse(categoria);
                        lista = db.Produto.Where(p => p.idCategoria == id).ToList();
                    }
                }
                else
                {
                    lista = db.Produto.Where(p => p.nomeProduto.ToLower().Contains(termo.ToLower())).ToList();
                }

                return View(lista);
            }
        }

        public ActionResult Produto(int id)
        {
            using (var db = new alphasupermarketEntities())
            {
                Produto produto = db.Produto.FirstOrDefault(p => p.idProduto == id);

                return View(produto);
            }
        }

        public ActionResult Carrinho()
        {
            return View(RecuperaCarrinhoSessao());
        }

        public ActionResult Error()
        {
            return View();
        }

        #region Login

        public ActionResult Login()
        {
            return View(new Cliente());
        }

        [HttpPost]
        public ActionResult Login(string usuario, string senha, string returnUrl)
        {
            Cliente cliente = new Database.Cliente();

            if (ModelState.IsValid)
            {
                cliente = ValidaLogin(usuario, senha);

                if (cliente != null)
                {
                    FormsAuthentication.SetAuthCookie(cliente.emailCliente, false);

                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    cliente = new Database.Cliente();

                    ModelState.AddModelError("", "Email ou Senha inválidos");
                }
            }

            return View(cliente);
        }

        #endregion

        #region Logout

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Métodos secundários

        private Cliente ValidaLogin(string usuario, string senha)
        {
            using (var db = new alphasupermarketEntities())
            {
                return db.Cliente.FirstOrDefault(c => c.emailCliente == usuario && c.senhaCliente == senha);
            }
        }

        public JsonResult InsereProdutoCarrinho(int produtoId, int quantidade)
        {
            using (var db = new alphasupermarketEntities())
            {
                List<Produto> carrinho = RecuperaCarrinhoSessao();

                var produto = db.Produto.FirstOrDefault(p => p.idProduto == produtoId);

                if (produto != null)
                {
                    carrinho = InsereProdutoSessao(produto, quantidade);
                }

                return Json(new
                {
                    totalProdutos = carrinho.Count,
                    valorTotal = carrinho.Sum(c => (c.precProduto - c.descontoPromocao.Value)).ToString("n2")
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ExcluirItemCarrinho(int produtoId)
        {
            RemoveProdutoSessao(produtoId);

            return RedirectToAction("Carrinho");
        }

        public static List<Produto> MaisVendidos()
        {
            using (var db = new alphasupermarketEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                var ids4MaisVendidos = db.ItemPedido.Include("Produto").GroupBy(p => p.idProduto).Select(gr => new { id = gr.Key, count = gr.Count() }).OrderByDescending(p => p.count).Take(4).Select(g => g.id).ToList();

                return db.Produto.Where(p => ids4MaisVendidos.Contains(p.idProduto)).ToList();
            }
        }

        public static List<Categoria> MaisCategorias()
        {
            using (var db = new alphasupermarketEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                int[] categoriasUsadas = { 1, 2, 3, 4, 5, 6 };

                return db.Categoria.Where(c => !categoriasUsadas.Contains(c.idCategoria)).ToList();
            }
        }

        #endregion
    }
}