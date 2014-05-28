using PI3.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PI3.Controllers
{
    public class BaseController : Controller
    {
		public ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			else
			{
				return RedirectToAction("Index", "Home");
			}
		}

		public List<Produto> InsereProdutoSessao(Produto produto, int quantidade)
		{
			List<Produto> carrinho = RecuperaCarrinhoSessao();

			for (int i = 0; i < quantidade; i++)
			{
				carrinho.Add(produto);
			}

			Session["carrinho"] = carrinho;

			return carrinho;
		}

        public void AtualizaQuantidadeSessao(Produto produto, int quantidadeTotal)
        {
            List<Produto> carrinho = RecuperaCarrinhoSessao();

            List<Produto> novaLista = carrinho.Where(l => l.idProduto != produto.idProduto).ToList();

            for (int i = 0; i < quantidadeTotal; i++)
            {
                novaLista.Add(produto);
            }

            Session["carrinho"] = novaLista;
        }

		public List<Produto> RecuperaCarrinhoSessao()
		{
			List<Produto> carrinho = new List<Produto>();

			if (Session["carrinho"] != null)
				carrinho = Session["carrinho"] as List<Produto>;

			return carrinho;
		}

		public void RemoveProdutoSessao(int produtoId)
		{
			List<Produto> lista = RecuperaCarrinhoSessao();

			List<Produto> novaLista = lista.Where(l => l.idProduto != produtoId).ToList();

			Session["carrinho"] = novaLista;
		}

        public void LimparCarrinho()
        {
            Session["carrinho"] = null;
        }
	}
}