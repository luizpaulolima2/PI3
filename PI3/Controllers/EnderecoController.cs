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

                        TempData["mensagem"] = "Endreço alterado com sucesso !";
                    }
                    catch (Exception)
                    {
                        TempData["mensagem"] = "Falha ao alterar o endereço.";
                    }
                }

                return RedirectToAction("Index");
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

                    TempData["mensagem"] = "Endreço adicionado com sucesso !";

                    return RedirectToAction("Index");
                }
            }

            return View(model);
        }

        public ActionResult Excluir(int id)
        {
            using (var db = new alphasupermarketEntities())
            {
                var endereco = db.Endereco.FirstOrDefault(c => c.idEndereco == id);

                try
                {
                    db.Endereco.Remove(endereco);
                    db.SaveChanges();

                    TempData["mensagem"] = "Endreço excluído com sucesso !";
                }
                catch (Exception)
                {
                    TempData["mensagem"] = "Erro ao excluir endereço";
                }

                return RedirectToAction("Index");
            }
        }
    }
}