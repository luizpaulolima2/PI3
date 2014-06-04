using PI3.Database;
using PI3.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PI3.Controllers
{
    [Authorize]
    public class ContaController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Cadastrar()
        {
            return View(new Cliente());
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Cadastrar(Cliente cliente, string returnUrl)
        {
            // removemos o erro do recebeNewsLetter, idCliente e data
            ModelState.Remove("recebeNewsLetter");
            ModelState.Remove("idCliente");
            ModelState.Remove("dtNascCliente");

            using (var db = new alphasupermarketEntities())
            {
                // Validamos se o email já existe
                var clienteComEmail = db.Cliente.FirstOrDefault(c => c.emailCliente == cliente.emailCliente);
                if (clienteComEmail != null && clienteComEmail.idCliente != cliente.idCliente)
                    ModelState.AddModelError("", "Email já cadastrado.");

                // Validamos a data
                if (cliente.dtNascCliente == null)
                    ModelState.AddModelError("", "Data inválida.");

                // valida a senha
                if (String.IsNullOrEmpty(cliente.senhaCliente))
                {
                    ModelState.AddModelError("", "Senha inválida.");
                }
                if (cliente.senhaCliente != Request.Form["confirmarSenha"])
                {
                    ModelState.AddModelError("", "Senhas não conferem.");
                }
            }

            if (ModelState.IsValid)
            {
                using (var db = new alphasupermarketEntities())
                {
                    try
                    {
                        // se o recebeNewsLetter esta presente no form
                        if (Request.Form["recebeNewsLetter"] != null && Request.Form["recebeNewsLetter"] == "on")
                            cliente.recebeNewsLetter = true;
                        else
                            cliente.recebeNewsLetter = false;

                        if (cliente.idCliente == 0)
                        {
                            db.Cliente.Add(cliente);
                        }
                        else
                        {
                            try
                            {
                                db.Entry<Cliente>(cliente).State = System.Data.Entity.EntityState.Modified;
                            }
                            catch (Exception)
                            {
                            }
                        }

                        db.SaveChanges();

                        // Loga o cliente no sistema
                        FormsAuthentication.SetAuthCookie(cliente.emailCliente, false);

                        TempData["sucesso"] = "Cadastro salvo com sucesso !";

                        return RedirectToLocal(returnUrl);
                    }
                    catch (Exception ex)
                    {
                        // Adiciona o erro no model
                        ModelState.AddModelError("", ex.Message);
                    }
                }
            }

            return View(cliente);
        }

        public ActionResult DadosCadastrais()
        {
            using (var db = new alphasupermarketEntities())
            {
                string email = User.Identity.Name;
                var cliente = db.Cliente.FirstOrDefault(c => c.emailCliente == email);

                return View(cliente);
            }
        }

        public ActionResult MeusPedidos()
        {
            using (var db = new alphasupermarketEntities())
            {
                string email = User.Identity.Name;
                var cliente = db.Cliente.Include("Pedido").Include("Pedido.StatusPedido").Include("Pedido.TipoPagamento").Include("Pedido.ItemPedido").Include("Pedido.ItemPedido.Produto").Include("Endereco").FirstOrDefault(c => c.emailCliente == email);
                return View(cliente);
            }
        }

        public ActionResult FinalizarCompra()
        {
            var lista = RecuperaCarrinhoSessao();

            if (lista.Count == 0)
                return RedirectToAction("Index", "Home");

            if (Request.Form.Count > 0)
            {
                foreach (var item in lista)
                {
                    AtualizaQuantidadeSessao(item, int.Parse(Request.Form["quantidade[" + item.idProduto + "]"]));
                }
            }

            ViewBag.carrinho = RecuperaCarrinhoSessao();

            List<SelectListItem> enderecos = new List<SelectListItem>();
            List<SelectListItem> pagamentos = new List<SelectListItem>();

            using (var db = new alphasupermarketEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                string email = User.Identity.Name;

                var cliente = db.Cliente.Include("endereco").FirstOrDefault(c => c.emailCliente == email);

                foreach (var endereco in cliente.Endereco.ToList())
                {
                    SelectListItem item = new SelectListItem();
                    item.Selected = false;
                    item.Text = endereco.nomeEndereco;
                    item.Value = endereco.idEndereco.ToString();

                    enderecos.Add(item);
                }

                pagamentos.Add(new SelectListItem() { Selected = false, Text = "Cartão de Crédito", Value = "1" });
            }

            ViewBag.enderecos = enderecos;
            ViewBag.pagamentos = pagamentos;

            return View();
        }

        [HttpPost]
        public ActionResult Finalizar()
        {
            Pedido pedido = new Pedido();

            using (var db = new alphasupermarketEntities())
            {
                try
                {
                    string email = User.Identity.Name;

                    var cliente = db.Cliente.Include("endereco").FirstOrDefault(c => c.emailCliente == email);

                    pedido.idCliente = cliente.idCliente;
                    pedido.idStatus = (int)StatusTransacaoEnum.AguardandoAprovacao;
                    pedido.dataPedido = DateTime.Now;
                    pedido.idTipoPagto = byte.Parse(Request.Form["pagamento"].ToString());
					pedido.idEndereco = int.Parse(Request.Form["endereco"].ToString());

                    db.Pedido.Add(pedido);
                    db.SaveChanges();

                    var carrinho = RecuperaCarrinhoSessao();

                    foreach (var item in carrinho.Distinct())
                    {
                        ItemPedido ip = new ItemPedido();
                        ip.idProduto = item.idProduto;
                        ip.idPedido = pedido.idPedido;
                        ip.qtdProduto = Convert.ToInt16(carrinho.Count(c => c.idProduto == item.idProduto));
                        ip.precoVendaItem = item.precProduto;

                        db.ItemPedido.Add(ip);
                    }

                    db.SaveChanges();

                    TempData["mensagem"] = "Obrigado por comprar no Alpha Supermarket ! <br /> A partir de agora qualquer atualização do seu pedido será comunicado via email.";
                }
                catch (Exception)
                {
                    TempData["erro"] = "Falha ao finalizar compra.";
                }
            }

            LimparCarrinho();

            return View();
        }

        public ActionResult CancelarPedido(int id)
        {
            using (var db = new alphasupermarketEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                var pedido = db.Pedido.FirstOrDefault(p => p.idPedido == id && p.idStatus <= 2);

                if (pedido != null)
                {
                    pedido.idStatus = (int)StatusTransacaoEnum.Cancelado;

                    db.SaveChanges();
                }
            }

            return RedirectToAction("MeusPedidos");
        }

        public  ActionResult DetalhesPedido(int id)
        {
            using (var db = new alphasupermarketEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                var pedido = db.Pedido.Include("ItemPedido").Include("ItemPedido.Produto").FirstOrDefault(p => p.idPedido == id);
                
                return View(pedido.ItemPedido.ToList());
            }
        }
    }
}