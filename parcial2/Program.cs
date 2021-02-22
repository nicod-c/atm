using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace parcial2
{
    class Moneda
    {
        public string codigoIso;
        public string nombre;
        public decimal ValorRelativoAlARS;

        public Moneda(string cod, string nombre, decimal valorRel)
        {
            this.codigoIso = cod;
            this.nombre = nombre;
            this.ValorRelativoAlARS = valorRel;
        }
    }

    class CajaAhorro
    {
        public string numero;
        public string cbu;
        public string alias;
        public Moneda moneda;
        public decimal descubierto;
        public decimal balance;

        public CajaAhorro(string num, string cbu, string alias, Moneda m, decimal desc, decimal bal)
        {
            this.numero = num;
            this.cbu = cbu;
            this.alias = alias;
            this.moneda = m;
            this.descubierto = desc;
            this.balance = bal;
        }
    }

    class Cliente
    {
        public string nombre;
        public string apellido;
        public string dni;
        public string nroCliente;
        public List<CajaAhorro> CajasAhorro;

        public Cliente(string nom, string ape, string dni, string nro, List<CajaAhorro> cas)
        {
            this.nombre = nom;
            this.apellido = ape;
            this.dni = dni;
            this.nroCliente = nro;
            this.CajasAhorro = cas;
        }
    }

    class TarjetaDebito
    {
        public Cliente cliente;
        public string numero;
        public DateTime vencimiento;
        public bool activa;
        public string clave;

        public TarjetaDebito()
        { }

        public TarjetaDebito(Cliente c, string nro, DateTime vto, bool a, string pass)
        {
            this.cliente = c;
            this.numero = nro;
            this.vencimiento = vto;
            this.activa = a;
            this.clave = pass;
        }

    }

    class BaseDatosCajero
    {
        public List<TarjetaDebito> BdTarjetas;
        public decimal BdFondosArs = 550255.50m;
        public decimal BdFondosUsd = 20000m;

        public BaseDatosCajero(List<TarjetaDebito> td)
        {
            this.BdTarjetas = td;

        }
    }

    class Program
    {
        static void MostrarPantallaPrincipal(TarjetaDebito tarjeta, BaseDatosCajero BdTarjetas)
        {
            int opcion = -1;

            MostrarOpcionesPp(tarjeta, ref opcion);

            switch (opcion)
            {
                case 0:
                    Console.Clear();
                    MostrarPantallaExtraccion(ref tarjeta, BdTarjetas);
                    break;
                case 1:
                    Console.Clear();
                    MostrarPantallaTransferencia(tarjeta, BdTarjetas);
                    break;
                case 2:
                    Console.Clear();
                    MostrarPantallaInfCbu(tarjeta, BdTarjetas);
                    break;
                case 3:
                    Console.Clear();
                    MostrarPantallaCambioPass(tarjeta, BdTarjetas);
                    break;
                case 4:
                    Console.Clear();
                    MostrarPantallaBienvenida(BdTarjetas);
                    break;
            }
        }

        static void MostrarOpcionesPp(TarjetaDebito tarjeta, ref int opcion)
        {
            Console.Clear();
            Console.WriteLine($"Bienvenido {tarjeta.cliente.nombre + " " + tarjeta.cliente.apellido}, ¿que desea hacer hoy?");
            Console.WriteLine("0 - Retirar dinero");
            Console.WriteLine("1 - Transferir dinero");
            Console.WriteLine("2 - Ver CBU");
            Console.WriteLine("3 - Cambiar clave");
            Console.WriteLine("4 - Salir");
            opcion = PedirValorInt("Su elección: ");
        }

        static bool EsTarjetaValida(ref TarjetaDebito tarjeta, BaseDatosCajero BdTarjetas)
        {

            for (int i = 0; i < BdTarjetas.BdTarjetas.Count; i++)
            {
                if (BdTarjetas.BdTarjetas[i].numero == tarjeta.numero && BdTarjetas.BdTarjetas[i].clave == tarjeta.clave)
                {
                    tarjeta = BdTarjetas.BdTarjetas[i];
                    return true;
                }
            }

            return false;
        }

        static bool EsNumeroTarjeta(ref TarjetaDebito tarjeta, BaseDatosCajero BdTarjetas)
        {
            for (int i = 0; i < BdTarjetas.BdTarjetas.Count; i++)
            {
                if (BdTarjetas.BdTarjetas[i].numero == tarjeta.numero)
                {
                    tarjeta.numero = BdTarjetas.BdTarjetas[i].numero;
                    return true;
                }
            }

            return false;
        }

        static bool EsClaveTarjeta(ref TarjetaDebito tarjeta, BaseDatosCajero BdTarjetas)
        {
            for (int i = 0; i < BdTarjetas.BdTarjetas.Count; i++)
            {
                if (BdTarjetas.BdTarjetas[i].clave == tarjeta.clave)
                {
                    tarjeta.clave = BdTarjetas.BdTarjetas[i].clave;
                    return true;
                }
            }

            return false;
        }

        static TarjetaDebito PedirDatosIngreso()
        {
            TarjetaDebito tarjeta = new TarjetaDebito();

            Console.Write("Ingrese Número de tarjeta: ");
            tarjeta.numero = Console.ReadLine();
            Console.Write("Ingrese Clave: ");
            tarjeta.clave = Console.ReadLine();

            return tarjeta;
        }

        static void DesactivarTarjeta(ref TarjetaDebito tarjeta)
        {
            tarjeta.activa = false;
        }

        static bool EsTarjetaVencida(TarjetaDebito tarjeta, BaseDatosCajero BdTarjetas)
        {
            if (EsTarjetaValida(ref tarjeta, BdTarjetas))
            {
                if (tarjeta.activa == true || tarjeta.vencimiento > DateTime.Today)
                {
                    return false;
                }
            }

            return true;
        }


        static void MostrarPantallaBienvenida(BaseDatosCajero BdTarjetas)
        {
            Console.Clear();
            int count = 2;

            TarjetaDebito tarjeta = PedirDatosIngreso();

            if (EsTarjetaValida(ref tarjeta, BdTarjetas) && !EsTarjetaVencida(tarjeta, BdTarjetas))
            {
                MostrarPantallaPrincipal(tarjeta, BdTarjetas);
            }

            while (EsNumeroTarjeta(ref tarjeta, BdTarjetas) && !EsClaveTarjeta(ref tarjeta, BdTarjetas) && count > 0)
            {
                Console.WriteLine("Clave inválida, intente nuevamente");
                Console.WriteLine($"Tiene {count} intentos mas");
                Console.ReadKey();
                count--;
                MostrarPantallaBienvenida(BdTarjetas);
            }

            if (EsNumeroTarjeta(ref tarjeta, BdTarjetas) && count == 0)
            {
                DesactivarTarjeta(ref tarjeta);
                Console.WriteLine("Ha ingresado la clave incorrecta 3 veces, se ha desactivado su tarjeta");
                Console.ReadKey();
                MostrarPantallaBienvenida(BdTarjetas);
            }

            if (EsTarjetaVencida(tarjeta, BdTarjetas) || !EsNumeroTarjeta(ref tarjeta, BdTarjetas))
            {
                Console.WriteLine("Tarjeta inválida o vencida, intente nuevamente");
                Console.ReadKey();
                MostrarPantallaBienvenida(BdTarjetas);
            }
        }



        static CajaAhorro MostrarSeleccionCajaAhorro(List<CajaAhorro> cajas, TarjetaDebito tarjeta, BaseDatosCajero bd)
        {
            int eleccion = -1;
            
            Console.WriteLine($"Elija la caja de ahorro a utilizar: ");

            for (int i = 0; i < cajas.Count; i++)
            {
                CajaAhorro caja = cajas[i];
                Console.WriteLine($"{i} - {caja.moneda.codigoIso} - {caja.numero} - Saldo disponible: ${caja.balance}");
            }

            Console.WriteLine($"{cajas.Count} - Cancelar");
            eleccion = PedirValorIntClampeado("Su eleccion:", 0, cajas.Count);

            if (eleccion == cajas.Count)
            {
                MostrarPantallaPrincipal(tarjeta, bd);
            }

            return cajas[eleccion];
        }

    

        static int PedirValorInt(string mensaje)
        {
            int rv = 0;

            Console.Write(mensaje);
            while (!int.TryParse(Console.ReadLine(), out rv))
            {
                Console.WriteLine("Valor invalido.");
                Console.Write(mensaje);
            }
            return rv;
        }

        static decimal PedirValorDecimal(string mensaje)
        {
            decimal rv = 0;

            Console.Write(mensaje);
            while (!decimal.TryParse(Console.ReadLine(), out rv))
            {
                Console.WriteLine("Valor invalido.");
                Console.Write(mensaje);
            }

            return rv;
        }

        static int PedirValorIntClampeado(string mensaje, int cotaInferior, int cotaSuperior)
        {
            int rv = PedirValorInt(mensaje);

            while (rv < cotaInferior || rv > cotaSuperior)
            {
                Console.WriteLine("Valor invalido.");
                rv = PedirValorInt(mensaje);
            }

            return rv;
        }

        static decimal PedirValorDecimalClampeado(string mensaje, decimal cotaInferior)
        {
            decimal rv = PedirValorDecimal(mensaje);

            while (rv < cotaInferior)
            {
                Console.WriteLine("Valor invalido.");
                rv = PedirValorDecimal(mensaje);
            }

            return rv;
        }

        static void MostrarPantallaExtraccion(ref TarjetaDebito tarjeta, BaseDatosCajero bd)
        {
            CajaAhorro caja = MostrarSeleccionCajaAhorro(tarjeta.cliente.CajasAhorro, tarjeta, bd);
            decimal extraccion = -1;
            int index = 0;

            for (int i = 0; i < tarjeta.cliente.CajasAhorro.Count; i++)
            {
                if (caja == tarjeta.cliente.CajasAhorro[i])
                {
                    Console.WriteLine($"¿Cuánto quiere retirar de la caja de ahorro {caja.moneda.codigoIso} - {caja.numero} ? (Fondos: {caja.balance} Descubierto: {caja.descubierto})");
                    index = i;
                    caja = tarjeta.cliente.CajasAhorro[i];
                }
            }

            if (caja.moneda.codigoIso == "ARS")
            { 
                extraccion = PedirValorDecimalClampeado("Ingrese el monto (0 para cancelar): ", 0);

                while (!CajaAhorro_PuedeExtraer(extraccion, caja))
                {
                    Console.WriteLine("Fondos insuficientes en caja de ahorro");
                    extraccion = PedirValorDecimalClampeado("Ingrese el monto (0 para cancelar): ", 0);
                }

                while (!Cajero_ConFondos(extraccion, bd.BdFondosArs))
                {
                    Console.WriteLine("Cajero sin fondos suficientes, ingrese un monto menor");
                    extraccion = PedirValorDecimalClampeado("Ingrese el monto (0 para cancelar): ", 0);
                }

                if (CajaAhorro_PuedeExtraer(extraccion, caja) && Cajero_ConFondos(extraccion, bd.BdFondosArs))
                {
                    CalcularSaldosExtraccion(ref bd.BdFondosArs, extraccion, tarjeta.cliente.CajasAhorro[index]);
                    Console.WriteLine($"Extracción de {caja.moneda.codigoIso}{extraccion} exitosa.");
                    Console.WriteLine($"Nuevos saldos de la Caja de ahorro {caja.moneda.codigoIso} - {tarjeta.cliente.CajasAhorro[index].numero}" +
                        $" (Fondos: {tarjeta.cliente.CajasAhorro[index].balance} Descubierto: {tarjeta.cliente.CajasAhorro[index].descubierto}");
                    extraccion = PedirValorDecimalClampeado("Presione 0 para continuar: ", 0);
                }
            }

            if (caja.moneda.codigoIso == "USD")
            {
                extraccion = PedirValorDecimalClampeado("Ingrese el monto (0 para cancelar): ", 0);

                while (!CajaAhorro_PuedeExtraer(extraccion, caja))
                {
                    Console.WriteLine("Fondos insuficientes en caja de ahorro");
                    extraccion = PedirValorDecimalClampeado("Ingrese el monto (0 para cancelar): ", 0);
                }

                while (!Cajero_ConFondos(extraccion, bd.BdFondosUsd))
                {
                    Console.WriteLine("Cajero sin fondos suficientes, ingrese un monto menor");
                    extraccion = PedirValorDecimalClampeado("Ingrese el monto (0 para cancelar): ", 0);
                }

                if (CajaAhorro_PuedeExtraer(extraccion, caja) && Cajero_ConFondos(extraccion, bd.BdFondosUsd))
                {
                    CalcularSaldosExtraccion(ref bd.BdFondosUsd, extraccion, tarjeta.cliente.CajasAhorro[index]);
                    Console.WriteLine($"Extracción de {caja.moneda.codigoIso}{extraccion} exitosa.");
                    Console.WriteLine($"Nuevos saldos de la Caja de ahorro {caja.moneda.codigoIso} - {tarjeta.cliente.CajasAhorro[index].numero}" +
                        $" (Fondos: {tarjeta.cliente.CajasAhorro[index].balance} Descubierto: {tarjeta.cliente.CajasAhorro[index].descubierto}");
                    extraccion = PedirValorDecimalClampeado("Presione 0 para continuar: ", 0);
                }
            }

            if (extraccion == 0)
            {
                MostrarPantallaPrincipal(tarjeta, bd);
            }
        }


        static void MostrarPantallaTransferencia(TarjetaDebito tarjeta, BaseDatosCajero bd)
        {
            MostrarSeleccionCajaAhorro(tarjeta.cliente.CajasAhorro, tarjeta, bd);
        }

        static bool CajaAhorro_PuedeExtraer(decimal extraccion, CajaAhorro caja)
        {
            if (extraccion > caja.balance + caja.descubierto)
            {         
                return false;
            }

            return true;
        }

        static bool Cajero_ConFondos(decimal extraccion, decimal fondosCajero)
        {
            if (extraccion > fondosCajero)
            {
                return false;
            }

            return true;
        }

        static void CalcularSaldosExtraccion(ref decimal fondosCajero, decimal extraccion, CajaAhorro caja)
        {
            fondosCajero -= extraccion;

            if (caja.balance > extraccion)
            {
                caja.balance -= extraccion;
            }

            else if (caja.balance - extraccion < 0)
            {
                caja.descubierto -= (extraccion - caja.balance);
                caja.balance = 0;
            }
        }

        static void MostrarPantallaInfCbu(TarjetaDebito tarjeta, BaseDatosCajero bd)
        {
            CajaAhorro caja = MostrarSeleccionCajaAhorro(tarjeta.cliente.CajasAhorro, tarjeta, bd);

            Console.WriteLine($"El CBU de la caja de ahorro {caja.moneda.codigoIso} - {caja.numero} es:");
            Console.WriteLine($"{caja.cbu}");
            Console.WriteLine("Presione Enter para salir");
            Console.ReadLine();
            MostrarPantallaPrincipal(tarjeta, bd);
        }

        static void MostrarPantallaCambioPass(TarjetaDebito tarjeta, BaseDatosCajero bd)
        {
            string ClaveActual;
            string NuevaClave;
            string NuevaClaveTemp;

            Console.Write("Indique su clave actual: ");
            ClaveActual = Console.ReadLine();

            Console.Write("Indique una nueva clave de 4 dígitos, diferente a la actual: ");
            NuevaClaveTemp = (Console.ReadLine());

            Console.Write("Repita la nueva clave: ");
            NuevaClave = Console.ReadLine();

            if (ClaveActual != tarjeta.clave)
            {
                Console.Write("Clave inválida");
                Console.ReadKey();
                MostrarPantallaPrincipal(tarjeta, bd);
            }
            else if (NuevaClaveTemp != NuevaClave || NuevaClave == tarjeta.clave)
            {
                Console.WriteLine("La Nueva clave no es válida");
                Console.ReadKey();
                MostrarPantallaPrincipal(tarjeta, bd);
            }
            else
                CambiarClave(NuevaClave, ref tarjeta, ref bd);
                Console.WriteLine("Cambio de clave realizado con éxito");
                Console.ReadKey();
                MostrarPantallaPrincipal(tarjeta, bd);
        }

        static void CambiarClave(string clave, ref TarjetaDebito tarjeta, ref BaseDatosCajero bd)
        {
            tarjeta.clave = clave;

            for (int i = 0; i < bd.BdTarjetas.Count; i++)
            {
                if (bd.BdTarjetas[i].numero == tarjeta.numero)
                {
                    bd.BdTarjetas[i].clave = clave;
                }
            }
        }

        static void Main(string[] args)
        {
            #region DB_CAJERO
            List<TarjetaDebito> tarjetas = new List<TarjetaDebito>();
            BaseDatosCajero BdTarjetas = new BaseDatosCajero(tarjetas);
                    

            List<CajaAhorro> cuentasCuno = new List<CajaAhorro>();
            List<CajaAhorro> cuentasCdos = new List<CajaAhorro>();
            List<CajaAhorro> cuentasCtres = new List<CajaAhorro>();
            List<CajaAhorro> cuentasCcuatro = new List<CajaAhorro>();
 
            Moneda Pesos = new Moneda("ARS", "Peso", 0.013m);
            Moneda Dolares = new Moneda("USD", "Dolar", 78.12m);

            Cliente clienteUno = new Cliente("Nicolas", "Cordoba", "32743499", "0001", cuentasCuno);
            Cliente clienteDos = new Cliente("Daniela", "Diaz", "32125217", "0002", cuentasCdos);
            Cliente clienteTres = new Cliente("Juan", "Perez", "24543234", "0003", cuentasCtres);
            Cliente clienteCuatro = new Cliente("Maria", "Gonzalez", "28444543", "0004", cuentasCcuatro);

            CajaAhorro caUno = new CajaAhorro("123456789", "2517433511100053655043", "Nico.C", Pesos, 0m, 15500m);
            CajaAhorro caUnoP = new CajaAhorro("1234567890", "2517433511100053655044", "Nico.C.P", Pesos, 1000m, 551200m);
            CajaAhorro caUnoUsd = new CajaAhorro("123456780", "2517433511100053655142", "Nico.C.Usd", Dolares, 0m, 150m);
            CajaAhorro caDos = new CajaAhorro("987654321", "3876116011100057554247", "Dani.Diaz", Pesos, 500m, 35000m);
            CajaAhorro caTres = new CajaAhorro("123498765", "5625623511100099967696", "PerezJ", Pesos, 300m, 21200m);
            CajaAhorro caCuatroP = new CajaAhorro("987612345", "3153794611100001345765", "MGonzalez", Pesos, 0m, 123245.52m);
            CajaAhorro caCuatro = new CajaAhorro("087612345", "3153794611100001345766", "MGonzalez.B", Pesos, 0m, 25433.22m);
            CajaAhorro caCinco = new CajaAhorro("111333444", "3822175811100051681145", null, Pesos, 0m, 0m);
          
            TarjetaDebito tdUno = new TarjetaDebito(clienteUno, "1111", new DateTime(2023 - 07), true, "1234");
            TarjetaDebito tdDos = new TarjetaDebito(clienteDos, "2222", new DateTime(2027 - 08), true, "5678");
            TarjetaDebito tdTres = new TarjetaDebito(clienteTres, "3333", new DateTime(2023 - 08), true, "1111");
            TarjetaDebito tdCuatro = new TarjetaDebito(clienteCuatro, "4444", new DateTime(2022 - 12), true, "4321");
            TarjetaDebito tdCinco = new TarjetaDebito(clienteUno, "5555", new DateTime(2020 - 06), false, "1234");
  
            tarjetas.Add(tdUno);
            tarjetas.Add(tdDos);
            tarjetas.Add(tdTres);
            tarjetas.Add(tdCuatro);
            tarjetas.Add(tdCinco);

            clienteUno.CajasAhorro.Add(caUno);
            clienteUno.CajasAhorro.Add(caUnoUsd);
            clienteUno.CajasAhorro.Add(caUnoP);
            clienteDos.CajasAhorro.Add(caDos);
            clienteTres.CajasAhorro.Add(caTres);
            clienteCuatro.CajasAhorro.Add(caCuatroP);
            clienteCuatro.CajasAhorro.Add(caCuatro);

            #endregion

            MostrarPantallaBienvenida(BdTarjetas);


            Console.ReadKey();

        }
    }
}
