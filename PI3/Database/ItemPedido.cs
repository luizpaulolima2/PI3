
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace PI3.Database
{

using System;
    using System.Collections.Generic;
    
public partial class ItemPedido
{

    public int idProduto { get; set; }

    public int idPedido { get; set; }

    public short qtdProduto { get; set; }

    public decimal precoVendaItem { get; set; }



    public virtual Pedido Pedido { get; set; }

    public virtual Produto Produto { get; set; }

}

}
