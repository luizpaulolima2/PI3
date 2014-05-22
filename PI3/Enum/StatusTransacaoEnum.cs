using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PI3.Enum
{
    public enum StatusTransacaoEnum
    {
        Aberto = 1,
        AguardandoAprovacao = 2,
        EnviadoParaTransportadora = 3,
        Entregue = 4,
        Cancelado = 5
    }
}