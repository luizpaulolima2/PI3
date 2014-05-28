using PI3.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PI3.Controllers
{
    [Authorize]
    public class EnderecoController : BaseController
    {
        public ActionResult Index()
        {
            using (var db = new alphasupermarketEntities())
            {
                string email = User.Identity.Name;
                var cliente = db.Cliente.Include("Endereco").FirstOrDefault(c => c.emailCliente == email);

                return View(cliente.Endereco.ToList());
            }
        }

        public ActionResult Editar(int id)
        {
            using (var db = new alphasupermarketEntities())
            {
                var endereco = db.Endereco.FirstOrDefault(c => c.idEndereco == id);

                return View(endereco);
            }
        }

        [HttpPost]
        public ActionResult Editar(Endereco model)
        {
            using (var db = new alphasupermarketEntities())
            {
                if(ModelState.IsValid)
                {
                    try
                    {
                        db.Entry<Endereco>(model).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        TempData["sucesso"] = "Endreço alterado com sucesso !";
                    }
                    catch (Exception)
                    {
                        TempData["erro"] = "Falha ao alterar o endereço. Tente novamente.";
                    }
                }

				return RedirectToAction("Index", "Endereco", new { returnUrl = Request.QueryString["ReturnUrl"] });
            }
        }

        public ActionResult Adicionar()
        {
            return View(new Endereco());
        }

        [HttpPost]
        public ActionResult Adicionar(Endereco model)
        {
            // removemos as chaves da validação, pois nesse caso nao vamos ter esses dados no Adicionar
            ModelState.Remove("idCliente");
            ModelState.Remove("idEndereco");

            if(ModelState.IsValid)
            {
                using (var db = new alphasupermarketEntities())
                {
                    string email = User.Identity.Name;
                    var cliente = db.Cliente.FirstOrDefault(c => c.emailCliente == email);

                    model.idCliente = cliente.idCliente;

                    db.Endereco.Add(model);
                    db.SaveChanges();

                    TempData["sucesso"] = "Endreço adicionado com sucesso !";

                    return RedirectToAction("Index");
                }
            }

            return View(model);
        }

        public ActionResult Excluir(int id)
        {
            using (var db = new alphasupermarketEntities())
            {
                var endereco = db.Endereco.Include("Pedido").FirstOrDefault(c => c.idEndereco == id);

                if(endereco.Pedido.Count > 0)
                {
                    TempData["erro"] = "Operação cancelada. Existe um ou mais pedidos em aberto com esse endereço !";

                    return RedirectToAction("Index");
                }

                try
                {
                    db.Endereco.Remove(endereco);
                    db.SaveChanges();

                    TempData["sucesso"] = "Endreço excluído com sucesso !";
                }
                catch (Exception)
                {
                    TempData["erro"] = "Falha ao excluir endereço. Tente novamente.";
                }

                return RedirectToAction("Index");
            }
        }
    }
}