using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TreskelRest.Biz.Interfaces;
using TreskelRest.Biz.Models.Documentos;

namespace TreskelRest.Biz.Servicios
{
    public class SriComprobanteService : ISriComprobanteService
    {
        public bool createOnHd = true;

        public byte[] GenerarFacturaPdf(Models.Factura factura, Models.Documentos.Factura documento, string empresaNombre, string direccionMatriz, bool createOnHd=true)
        {
            InfoTributaria infoTributaria = documento.InfoTributaria;
            InfoFactura infoFactura = documento.InfoFactura;

            string numeroFactura = infoTributaria.Estab+ "-" + infoTributaria.PtoEmi + "-" + infoTributaria.Secuencial;

            QuestPDF.Settings.License = LicenseType.Community;

            var pdfDocument = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(8.5f));

                    // ===============================================
                    // 1. ENCABEZADO (LOGO + DATOS EMISOR Y FACTURA)
                    // ===============================================
                    page.Header().PaddingBottom(15).Row(row =>
                    {
                        row.RelativeItem(1f).Column(column =>
                        {
                            string rutaNoLogo = Path.Combine(AppContext.BaseDirectory, "Resources", "no-logo.png");

                            if (factura.logotipo == null){
                                if (File.Exists(rutaNoLogo))
                                {
                                    factura.logotipo = File.ReadAllBytes(rutaNoLogo);
                                }
                            }

                            if (factura.logotipo != null)
                                column.Item().Width(230).Height(70).Image(factura.logotipo);
                            else
                                column.Item().Width(230).Height(70).Background(Colors.Grey.Lighten3)
                                .AlignCenter().Text("SIN LOGOTIPO").FontSize(10).FontColor(Colors.Cyan.Accent2);

                            column.Item().PaddingVertical(2).Text(t => {
                                t.Span("Ruc: ").Bold().FontSize(9.5f);
                                t.Span(factura.Ruc).Bold().FontSize(9.5f);
                            });

                            column.Item().PaddingTop(4).Text($"{factura.RazonSocial}").Bold().FontSize(13f).AlignLeft();
                            
                            column.Item().PaddingVertical(2).Text(t => {
                                t.Span("Dirección Matriz: ").Bold().FontSize(8.5f);
                                t.Span(infoTributaria.DirMatriz).FontSize(8.5f);
                            });

                            column.Item().PaddingVertical(2).Text(t => {
                                t.Span("Direccion Local: ").Bold().FontSize(8.5f);
                                t.Span(infoFactura.DirEstablecimiento).FontSize(8.5f);
                            });
                            column.Item().PaddingVertical(2).Text(t =>
                            {
                                t.Span("Obligado a llevar contabilidad: ").Bold().FontSize(8.5f);
                                t.Span(infoFactura.ObligadoContabilidad).FontSize(8.5f); 
                            });

                            if (infoTributaria.ContribuyenteRimpe != null)
                                column.Item().PaddingVertical(2).Text(infoTributaria.ContribuyenteRimpe).FontSize(8.5f).Bold();
                            else
                                column.Item().PaddingVertical(2).Text("CONTRIBUYENTE RÉGIMEN RIMPE").FontSize(8.5f).Bold();
                        });

                        row.ConstantItem(20);

                        row.RelativeItem(1.1f).Border(1).BorderColor(Colors.Grey.Darken1).Padding(8).Column(column =>
                        {
                            column.Item().Text("FACTURA").Bold().FontSize(14).AlignCenter();
                            column.Item().PaddingTop(2).Text(t => {
                                t.Span("No: ").Bold().FontSize(10);
                                t.Span(numeroFactura).Bold().FontColor(Colors.Red.Medium).FontSize(10);
                            });

                            column.Item().PaddingTop(4).Text("Numero Autorización:").Bold().FontSize(8.5f);
                            column.Item().Text(factura.ClaveAcceso).FontSize(8f);

                            column.Item().PaddingTop(2).Text(t => {
                                t.Span("Fecha Autorización:").Bold().FontSize(8.5f);
                                t.Span(factura.FechaAutorizacion).FontSize(8.5f);
                            });

                            column.Item().PaddingTop(2).Text(t => { 
                                t.Span("Ambiente: ").Bold(); 
                                t.Span(factura.Ambiente).FontSize(8.5f); 
                            });
                            column.Item().Text(t => { 
                                t.Span("Emisión: ").Bold().FontSize(8.5f); 
                                t.Span("Normal").FontSize(8.5f); 
                            });
                            column.Item().Text("Clave Acceso").Bold().FontSize(8.5f);

                            // Código de barras nativo vectorial e idéntico al de la imagen
                            if (!string.IsNullOrEmpty(factura.ClaveAcceso) && factura.ClaveAcceso.Length == 49)
                            {
                                column.Item().PaddingTop(8).Height(35).AlignCenter().BarcodeCode128(factura.ClaveAcceso);
                                //column.Item().AlignCenter().Text(factura.ClaveAcceso).FontSize(7.5f);
                            }
                        });
                    });

                    // ================================================================
                    // 2. DATOS DEL CLIENTE (Encerrado entre dos líneas horizontales)
                    // ================================================================
                    page.Content().Column(mainCol =>
                    {
                        mainCol.Item()
                        .BorderTop(0.5f).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1)
                        .PaddingVertical(6).Row(row =>
                        {
                            row.RelativeItem(1.3f).Column(c =>
                            {
                                c.Item().Text(t => { t.Span("Razón social/Nombres Apellidos: ").Bold(); t.Span(factura.Cliente); });
                                c.Item().PaddingTop(2).Text(t => { t.Span("Fecha de emisión: ").Bold(); t.Span(infoFactura.FechaEmision); });
                                c.Item().PaddingTop(2).Text(t => { t.Span("Dirección: ").Bold(); t.Span(infoFactura.DireccionComprador ?? "S/D"); });
                            });
                            row.RelativeItem(0.7f).Column(c =>
                            {
                                c.Item().Text(t => { t.Span("Identificación: ").Bold(); t.Span(factura.ClienteNid); });
                            });
                        });

                        mainCol.Item().PaddingTop(15);

                        // ===============================================
                        // 3. CUERPO DEL COMPROBANTE (TABLA + TOTALES)
                        // ===============================================
                        mainCol.Item().Table(table => {
                            // Definición de las columnas
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1.0f); // Código
                                columns.RelativeColumn(1.0f); // Cantidad
                                columns.RelativeColumn(4.7f); // Descripción
                                columns.RelativeColumn(1.5f); // Precio Unitario
                                columns.RelativeColumn(1.3f); // Descuento
                                columns.RelativeColumn(1.5f); // Precio Total
                            });

                            // Definición de la cabeccera
                            table.Header(header =>
                            {
                                var estilo = TextStyle.Default.Bold().FontSize(9.5f);
                                header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Darken1).PaddingBottom(4).Text("Código").Style(estilo);
                                header.Cell().PaddingHorizontal(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).PaddingBottom(4).Text("Cantidad").Style(estilo).AlignRight();
                                header.Cell().PaddingHorizontal(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).PaddingBottom(4).Text("Descripción").Style(estilo);
                                header.Cell().PaddingHorizontal(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).PaddingBottom(4).Text("Precio Unitario").Style(estilo).AlignRight();
                                header.Cell().PaddingHorizontal(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).PaddingBottom(4).Text("Descuento").Style(estilo).AlignRight();
                                header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Darken1).PaddingBottom(4).Text("Precio Total").Style(estilo).AlignRight();
                            });

                            // Filas de los productos comprados (Ej: Hamburguesa Doble)
                            if (factura.facturaDetalle != null && factura.facturaDetalle.Count > 0)
                            {
                                foreach (var detalle in factura.facturaDetalle)
                                {
                                    string punit = string.Format("{0:0.00}", double.Parse(detalle.precioUnitario));
                                    string ptotal = string.Format("{0:0.00}", double.Parse(detalle.precioTotal));
                                    string pdcto = string.Format("{0:0.00}", double.Parse(detalle.descuento));

                                    table.Cell().PaddingVertical(5).Text(detalle.codigo);
                                    table.Cell().PaddingHorizontal(4).PaddingVertical(5).Text(detalle.cantidad).AlignRight();
                                    table.Cell().PaddingHorizontal(4).PaddingVertical(5).Text(detalle.descripcion);
                                    table.Cell().PaddingHorizontal(4).PaddingVertical(5).Text(string.IsNullOrEmpty(punit) ? "0.00" : punit).AlignRight();
                                    table.Cell().PaddingHorizontal(4).PaddingVertical(5).Text(string.IsNullOrEmpty(pdcto) ? "0.00" : pdcto).AlignRight();
                                    table.Cell().PaddingVertical(5).Text(string.IsNullOrEmpty(ptotal) ? "0.00" : ptotal).AlignRight();
                                }
                            }
                            else
                            {
                                foreach (var detalle in documento.Detalles.Detalle)
                                {
                                    table.Cell().PaddingVertical(5).Text(detalle.CodigoPrincipal);
                                    table.Cell().PaddingHorizontal(4).PaddingVertical(5).Text(detalle.Cantidad).AlignRight();
                                    table.Cell().PaddingHorizontal(4).PaddingVertical(5).Text(detalle.Descripcion);
                                    table.Cell().PaddingHorizontal(4).PaddingVertical(5).Text(detalle.PrecioUnitario == "0" ? "0.00": detalle.PrecioUnitario).AlignRight();
                                    table.Cell().PaddingHorizontal(4).PaddingVertical(5).Text(detalle.Descuento == "0" ? "0.00" : detalle.Descuento).AlignRight();
                                    table.Cell().PaddingVertical(5).Text(detalle.PrecioTotalSinImpuesto == "0" ? "0.00" : detalle.PrecioTotalSinImpuesto).AlignRight();
                                }
                            }

                            table.Cell().ColumnSpan(6).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(0).ExtendHorizontal();

                            // BLOQUE INFERIOR IZQUIERDO: Información Adicional (Sin recuadros pesados)
                            table.Cell().RowSpan(9).ColumnSpan(3).PaddingTop(12).PaddingRight(20).Column(col =>
                            {
                                col.Item().Text("Información Adicional").Bold().FontSize(9.5f);
                                if (factura.InformacionAdicional != null)
                                {
                                    foreach (var info in factura.InformacionAdicional)
                                    {
                                        col.Item().PaddingTop(2).Text(txt => {
                                            txt.Span($"{info.Nombre} : ").Bold().FontSize(8.5f);
                                            txt.Span(info.Valor).FontSize(8.5f);
                                        });
                                    }
                                }
                                else {
                                    foreach (var info in documento.InfoAdicional.CampoAdicional)
                                    {
                                        col.Item().PaddingTop(2).Text(txt => {
                                            txt.Span($"{info.Nombre} : ").Bold().FontSize(8.5f);
                                            txt.Span(info.Text).FontSize(8.5f);
                                        });
                                    }
                                }
                            });

                            /*------------------------------------------------------
                             * FORMA DE PAGO
                             -------------------------------------------------------*/
                            table.Cell().RowSpan(9).ColumnSpan(3).PaddingTop(12).PaddingRight(20).Column(col =>
                            {
                                col.Item().Text("Forma de Pago").Bold().FontSize(9.5f);
                                foreach (var pago in factura.formaDePago)
                                {
                                    col.Item().PaddingTop(2).Text(txt => {
                                        txt.Span($"{pago.tipo} : ").Bold().FontSize(8.5f);
                                        txt.Span($"{pago.valor}").FontSize(8.5f);
                                    });
                                }
                            });


                            var porcentajeIva = infoFactura.TotalConImpuestos.TotalImpuesto[0].DecoPorcentajeImpuesto();

                            var subSinIVA = documento.Detalles.Detalle
                                .Where(x => x.Impuestos.Impuesto.Sum(o => double.Parse(o.Valor)) == 0)
                                .Sum(o => double.Parse(o.PrecioTotalSinImpuesto));

                            var subTotal = documento.Detalles.Detalle
                                .Where(x => x.Impuestos.Impuesto.Sum(o => double.Parse(o.Valor)) > 0)
                                .Sum(o => double.Parse(o.PrecioTotalSinImpuesto));

                            var descuento = documento.Detalles.Detalle
                                .Sum(o => double.Parse(o.Descuento));

                            var total = double.Parse(infoFactura.ImporteTotal);

                            var iva = documento.InfoFactura.TotalConImpuestos.TotalImpuesto.Sum(x => double.Parse(x.Valor));

                            // BLOQUE INFERIOR DERECHO: Tabla de Acumuladores Estilo SRI
                            table.Cell().RowSpan(9).ColumnSpan(3).PaddingTop(12).AlignRight().Table(totalesTable =>
                            {
                                // Definimos la estructura interna de 2 columnas para controlar el ancho de los totales
                                totalesTable.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();   // Columna para las descripciones (Texto alineado a la izquierda por defecto)
                                    columns.ConstantColumn(70); // Columna fija para los valores numéricos alineados a la derecha
                                });

                                var borderGrisClaro = 0.5f;
                                var colorLinea = Colors.Grey.Lighten2;

                                // 1. Subtotal Con Iva
                                totalesTable.Cell()
                                    .BorderBottom(borderGrisClaro).BorderColor(colorLinea)
                                    .PaddingVertical(4).Text($"Subtotal {porcentajeIva}%").FontSize(8.5f);
                                totalesTable.Cell()
                                    .BorderBottom(borderGrisClaro).BorderColor(colorLinea)
                                    .PaddingVertical(4).Text(string.Format("{0:0.00}", subTotal)).AlignRight().FontSize(8.5f);

                                // 2. Subtotal 0%
                                totalesTable.Cell()
                                    .BorderBottom(borderGrisClaro).BorderColor(colorLinea)
                                    .PaddingVertical(4).Text("Subtotal 0%").FontSize(8.5f);
                                totalesTable.Cell()
                                    .BorderBottom(borderGrisClaro).BorderColor(colorLinea)
                                    .PaddingVertical(4).Text(string.Format("{0:0.00}", subSinIVA)).AlignRight().FontSize(8.5f);

                                // 3. Exento de IVA (Corregido "Excento")
                                totalesTable.Cell()
                                    .BorderBottom(borderGrisClaro).BorderColor(colorLinea)
                                    .PaddingVertical(4).Text("Subtotal Exento de IVA").FontSize(8.5f);
                                totalesTable.Cell()
                                    .BorderBottom(borderGrisClaro).BorderColor(colorLinea)
                                    .PaddingVertical(4).Text("0.00").AlignRight().FontSize(8.5f);

                                // 4. Total descuento
                                totalesTable.Cell()
                                    .BorderBottom(borderGrisClaro).BorderColor(colorLinea)
                                    .PaddingVertical(4).Text("Total descuento").FontSize(8.5f);
                                totalesTable.Cell()
                                    .BorderBottom(borderGrisClaro).BorderColor(colorLinea)
                                    .PaddingVertical(4).Text(string.Format("{0:0.00}", descuento)).AlignRight().FontSize(8.5f);

                                // 5. ICE
                                totalesTable.Cell()
                                    .BorderBottom(borderGrisClaro).BorderColor(colorLinea)
                                    .PaddingVertical(4).Text("ICE").FontSize(8.5f);
                                totalesTable.Cell()
                                    .BorderBottom(borderGrisClaro).BorderColor(colorLinea)
                                    .PaddingVertical(4).Text("0.00").AlignRight().FontSize(8.5f);

                                // 6. IVA
                                totalesTable.Cell()
                                    .BorderBottom(borderGrisClaro).BorderColor(colorLinea)
                                    .PaddingVertical(4).Text($"Valor IVA {porcentajeIva}%").FontSize(8.5f);
                                totalesTable.Cell()
                                    .BorderBottom(borderGrisClaro).BorderColor(colorLinea)
                                    .PaddingVertical(4).Text(string.Format("{0:0.00}", iva)).AlignRight().FontSize(8.5f);

                                // 7. Propina
                                totalesTable.Cell()
                                    .BorderBottom(borderGrisClaro).BorderColor(colorLinea)
                                    .PaddingVertical(4).Text("Propina").FontSize(8.5f);
                                totalesTable.Cell()
                                    .BorderBottom(borderGrisClaro).BorderColor(colorLinea)
                                    .PaddingVertical(4).Text("0.00").AlignRight().FontSize(8.5f);

                                // 8. VALOR TOTAL
                                totalesTable.Cell()
                                    .BorderBottom(borderGrisClaro).BorderColor(colorLinea)
                                    .PaddingTop(6).PaddingBottom(2).Text("VALOR TOTAL").Bold().FontSize(9f);
                                totalesTable.Cell()
                                    .BorderBottom(borderGrisClaro).BorderColor(colorLinea)
                                    .PaddingTop(6).PaddingBottom(2).Text(string.Format("{0:0.00}", total)).AlignRight().Bold().FontSize(9f);
                            });

                            var formaPagos = infoFactura.Pagos.Pago;

                            table.Cell().RowSpan(9).ColumnSpan(3).PaddingTop(12).PaddingRight(20).Column(col =>
                            {
                                col.Item().Text("Formas de Pago").Bold().FontSize(9.5f);

                                foreach (var pago in formaPagos)
                                {
                                    col.Item().PaddingTop(2).Text(txt => {
                                        txt.Span($"{getFormaPago(pago.FormaPago)}").Bold().FontSize(8.5f);
                                        txt.Span(pago.Total).FontSize(8.5f);
                                    });
                                }
                            });
                        });
                    });
                });
            });

            if (pdfDocument == null)
            {
                throw new InvalidOperationException("No se pudo inicializar la estructura del documento PDF.");
            }

            using (var stream = new System.IO.MemoryStream())
            {
                pdfDocument.GeneratePdf(stream);
                string path = generaPathArchivo(infoTributaria.NombreComercial, factura.ClaveAcceso, "Facturacion");
                File.WriteAllBytes(path, stream.ToArray());
                return stream.ToArray();
            }

            throw new NotImplementedException();
        }

        public byte[] GenerarRetencionPdf(Models.Retencion retencion, Models.Documentos.Retencion documento, string empresaNombre, string direccionMatriz, bool createOnHd = true)
        {
            InfoTributaria infoTributaria = documento.InfoTributaria;
            InfoCompRetencion infoCompRetencion = documento.InfoCompRetencion;
            ImpuestosRetencion impuestosRetencion = documento.Impuestos;
            InfoAdicional infoAdicional = documento.InfoAdicional;

            retencion.FechaAutorizacion = DateTime.Today.ToString("dd/MM/yyyy");

            string direccionRetencion = infoAdicional.CampoAdicional.Where(x => x.Nombre == "Dirección").Select(y => y.Text).FirstOrDefault();

            string numeroFactura = infoTributaria.Estab + "-" + infoTributaria.PtoEmi + "-" + infoTributaria.Secuencial;

            QuestPDF.Settings.License = LicenseType.Community;

            var pdfDocument = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(8.5f));

                    // ===============================================
                    // 1. ENCABEZADO (LOGO + DATOS EMISOR Y FACTURA)
                    // ===============================================
                    page.Header().PaddingBottom(15).Row(row =>
                    {
                        row.RelativeItem(1f).Column(column =>
                        {
                            string rutaNoLogo = Path.Combine(AppContext.BaseDirectory, "Resources", "no-logo.png");

                            if (retencion.logotipo == null)
                            {
                                if (File.Exists(rutaNoLogo))
                                {
                                    retencion.logotipo = File.ReadAllBytes(rutaNoLogo);
                                }
                            }

                            if (retencion.logotipo != null)
                                column.Item().Width(230).Height(70).Image(retencion.logotipo);
                            else
                                column.Item().Width(230).Height(70).Background(Colors.Grey.Lighten3)
                                .AlignCenter().Text("SIN LOGOTIPO").FontSize(10).FontColor(Colors.Cyan.Accent2);

                            column.Item().PaddingVertical(2).Text(t => {
                                t.Span("Ruc: ").Bold().FontSize(9.5f);
                                t.Span(retencion.Ruc).Bold().FontSize(9.5f);
                            });

                            column.Item().PaddingTop(4).Text($"{infoTributaria.RazonSocial}").Bold().FontSize(13f).AlignLeft();

                            column.Item().PaddingVertical(2).Text(t => {
                                t.Span("Dirección Matriz: ").Bold().FontSize(8.5f);
                                t.Span(infoTributaria.DirMatriz).FontSize(8.5f);
                            });

                            column.Item().PaddingVertical(2).Text(t => {
                                t.Span("Direccion Local: ").Bold().FontSize(8.5f);
                                t.Span(infoCompRetencion.DirEstablecimiento).FontSize(8.5f);
                            });
                            column.Item().PaddingVertical(2).Text(t =>
                            {
                                t.Span("Obligado a llevar contabilidad: ").Bold().FontSize(8.5f);
                                t.Span(infoCompRetencion.ObligadoContabilidad).FontSize(8.5f);
                            });

                            if (infoTributaria.ContribuyenteRimpe != null)
                                column.Item().PaddingVertical(2).Text(infoTributaria.ContribuyenteRimpe).FontSize(8.5f).Bold();
                            else
                                column.Item().PaddingVertical(2).Text("CONTRIBUYENTE RÉGIMEN RIMPE").FontSize(8.5f).Bold();
                        });

                        row.ConstantItem(20);

                        row.RelativeItem(1.1f).Border(1).BorderColor(Colors.Grey.Darken1).Padding(8).Column(column =>
                        {
                            column.Item().Text("COMPROBANTE DE RETENCIÓN").Bold().FontSize(14).AlignCenter();
                            column.Item().PaddingTop(2).Text(t => {
                                t.Span("No: ").Bold().FontSize(10);
                                t.Span(numeroFactura).Bold().FontColor(Colors.Red.Medium).FontSize(10);
                            });

                            column.Item().PaddingTop(4).Text("Numero Autorización:").Bold().FontSize(8.5f);
                            column.Item().Text(retencion.ClaveAcceso).FontSize(8f);

                            column.Item().PaddingTop(2).Text(t => {
                                t.Span("Fecha Autorización:").Bold().FontSize(8.5f);
                                t.Span(retencion.FechaAutorizacion).FontSize(8.5f);
                            });

                            column.Item().PaddingTop(2).Text(t => {
                                t.Span("Ambiente: ").Bold();
                                t.Span(retencion.Ambiente).FontSize(8.5f);
                            });
                            column.Item().Text(t => {
                                t.Span("Emisión: ").Bold().FontSize(8.5f);
                                t.Span("Normal").FontSize(8.5f);
                            });
                            column.Item().Text("Clave Acceso").Bold().FontSize(8.5f);

                            // Código de barras nativo vectorial e idéntico al de la imagen
                            if (!string.IsNullOrEmpty(retencion.ClaveAcceso) && retencion.ClaveAcceso.Length == 49)
                            {
                                column.Item().PaddingTop(8).Height(35).AlignCenter().BarcodeCode128(retencion.ClaveAcceso);
                            }
                        });
                    });

                    // ================================================================
                    // 2. DATOS DEL CLIENTE (Encerrado entre dos líneas horizontales)
                    // ================================================================
                    page.Content().Column(mainCol =>
                    {
                        mainCol.Item()
                        .BorderTop(0.5f).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1)
                        .PaddingVertical(6).Row(row =>
                        {
                            row.RelativeItem(1.3f).Column(c =>
                            {
                                c.Item().Text(t => { t.Span("Razón social/Nombres Apellidos: ").Bold(); t.Span(infoCompRetencion.RazonSocialSujetoRetenido); });
                                c.Item().PaddingTop(2).Text(t => { t.Span("Fecha de emisión: ").Bold(); t.Span(infoCompRetencion.FechaEmision); });
                                c.Item().PaddingTop(2).Text(t => { t.Span("Dirección: ").Bold(); t.Span(direccionRetencion ?? "S/D"); });
                            });
                            row.RelativeItem(0.7f).Column(c =>
                            {
                                c.Item().Text(t => { t.Span("Identificación: ").Bold(); t.Span(infoCompRetencion.IdentificacionSujetoRetenido); });
                            });
                        });
                        
                        mainCol.Item().PaddingTop(15);

                        // ===============================================
                        // 3. CUERPO DEL COMPROBANTE (TABLA + TOTALES)
                        // ===============================================
                        mainCol.Item().Table(table => {
                            // Definición de las columnas
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1.5f); // Comprobante
                                columns.RelativeColumn(2.0f); // Número
                                columns.RelativeColumn(2.0f); // Fecha Emisipon
                                columns.RelativeColumn(1.5f); // Ejercicio Fiscal
                                columns.RelativeColumn(1.5f); // Base Imponible
                                columns.RelativeColumn(1.5f); // Tipo Impuesto
                                columns.RelativeColumn(1.5f); // Porcentaje Retención
                                columns.RelativeColumn(1.5f); // Impuesto
                            });

                            // Definición de la cabeccera
                            table.Header(header =>
                            {
                                var estilo = TextStyle.Default.Bold().FontSize(9.5f);
                                header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Darken1).PaddingBottom(4).Text("Comprobante").Style(estilo);
                                header.Cell().PaddingHorizontal(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).PaddingBottom(4).Text("Número").Style(estilo).AlignRight();
                                header.Cell().PaddingHorizontal(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).PaddingBottom(4).Text("Fecha Emisión").Style(estilo);
                                header.Cell().PaddingHorizontal(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).PaddingBottom(4).Text("Ejercicio Fiscal").Style(estilo).AlignRight();
                                header.Cell().PaddingHorizontal(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).PaddingBottom(4).Text("Base Imponible").Style(estilo).AlignRight();
                                header.Cell().PaddingHorizontal(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).PaddingBottom(4).Text("Tipo Impuesto").Style(estilo).AlignRight();
                                header.Cell().PaddingHorizontal(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).PaddingBottom(4).Text("% Retención").Style(estilo).AlignRight();
                                header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Darken1).PaddingBottom(4).Text("Impuesto").Style(estilo).AlignRight();
                            });

                            if (retencion.retencionDetalle != null && retencion.retencionDetalle.Count > 0)
                            {
                                foreach (var detalle in retencion.retencionDetalle)
                                {
                                    string baseImpo = string.Format("{0:0.00}", double.Parse(detalle.baseImponible));
                                    string pctjRete = string.Format("{0:0.00}", double.Parse(detalle.porcentajeRetencion));
                                    string Impuesto = string.Format("{0:0.00}", double.Parse(detalle.valorRetenido));

                                    table.Cell().PaddingVertical(5).Text("FACTURA");
                                    table.Cell().PaddingHorizontal(4).PaddingVertical(5).Text(detalle.numeroFactura).AlignRight();
                                    table.Cell().PaddingHorizontal(4).PaddingVertical(5).Text(detalle.fechaEmision);
                                    table.Cell().PaddingHorizontal(4).PaddingVertical(5).Text(detalle.ejercicioFiscal);
                                    table.Cell().PaddingHorizontal(4).PaddingVertical(5).Text(string.IsNullOrEmpty(baseImpo) ? "0.00" : baseImpo).AlignRight();
                                    table.Cell().PaddingHorizontal(4).PaddingVertical(5).Text(detalle.tipoImpuesto).AlignRight();
                                    table.Cell().PaddingHorizontal(4).PaddingVertical(5).Text(string.IsNullOrEmpty(pctjRete) ? "0.00" : pctjRete).AlignRight();
                                    table.Cell().PaddingVertical(5).Text(string.IsNullOrEmpty(Impuesto) ? "0.00" : Impuesto).AlignRight();
                                }
                            }
                            else
                            {
                                foreach (var detalle in documento.Impuestos.Impuesto)
                                {
                                    string sustento = detalle.NumDocSustento;
                                    string nf1 = sustento.Substring(0, 3);
                                    string nf2 = sustento.Substring(3, 3);
                                    string nf3 = sustento.Substring(6);

                                    table.Cell().PaddingVertical(5).Text("FACTURA");
                                    table.Cell().PaddingHorizontal(4).PaddingVertical(5).Text(sustento).AlignRight();
                                    table.Cell().PaddingHorizontal(4).PaddingVertical(5).Text(detalle.FechaEmisionSustento);
                                    table.Cell().PaddingHorizontal(4).PaddingVertical(5).Text(infoCompRetencion.PeriodoFiscal).AlignRight();
                                    table.Cell().PaddingHorizontal(4).PaddingVertical(5).Text(detalle.BaseImponible).AlignRight();
                                    table.Cell().PaddingHorizontal(4).PaddingVertical(5).Text(detalle.Codigo == "1" ? "RENTA" : "IVA").AlignRight();
                                    table.Cell().PaddingHorizontal(4).PaddingVertical(5).Text(detalle.PorcentajeRetener).AlignRight();
                                    table.Cell().PaddingVertical(5).Text(detalle.Valor).AlignRight();
                                }
                            }

                            table.Cell().ColumnSpan(8).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(0).ExtendHorizontal();

                            // BLOQUE INFERIOR IZQUIERDO: Información Adicional (Sin recuadros pesados)
                            table.Cell().RowSpan(9).ColumnSpan(3).PaddingTop(12).PaddingRight(20).Column(col =>
                            {
                                col.Item().Text("Información Adicional").Bold().FontSize(9.5f);
                                if (documento.InfoAdicional != null)
                                {
                                    foreach (var info in documento.InfoAdicional.CampoAdicional)
                                    {
                                        col.Item().PaddingTop(2).Text(txt => {
                                            txt.Span($"{info.Nombre} : ").Bold().FontSize(8.5f);
                                            txt.Span(info.Text).FontSize(8.5f);
                                        });
                                    }
                                }
                            });
                        });
                    });
                });
            });

            if (pdfDocument == null)
            {
                throw new InvalidOperationException("No se pudo inicializar la estructura del documento PDF.");
            }

            using (var stream = new System.IO.MemoryStream())
            {
                pdfDocument.GeneratePdf(stream);
                string path = generaPathArchivo(infoTributaria.NombreComercial, retencion.ClaveAcceso, "Retencion");
                File.WriteAllBytes(path, stream.ToArray());
                return stream.ToArray();
            }

            throw new NotImplementedException();
        }

        public byte[] GenerarNotaCreditoPdf(Models.NotaCredito notacredito, Models.Documentos.NotaCredito documento, string empresaNombre, string direccionMatriz, bool createOnHd = true)
        {
            InfoTributaria infoTributaria = documento.InfoTributaria;
            InfoNotaCredito infoNotaCredito = documento.InfoNotaCredito;

            string numeroFactura = infoTributaria.Estab + "-" + infoTributaria.PtoEmi + "-" + infoTributaria.Secuencial;

            QuestPDF.Settings.License = LicenseType.Community;

            var pdfDocument = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(8.5f));

                    // ===============================================
                    // 1. ENCABEZADO (LOGO + DATOS EMISOR Y FACTURA)
                    // ===============================================
                    page.Header().PaddingBottom(15).Row(row =>
                    {
                        row.RelativeItem(1f).Column(column =>
                        {
                            string rutaNoLogo = Path.Combine(AppContext.BaseDirectory, "Resources", "no-logo.png");

                            if (notacredito.logotipo == null)
                            {
                                if (File.Exists(rutaNoLogo))
                                {
                                    notacredito.logotipo = File.ReadAllBytes(rutaNoLogo);
                                }
                            }

                            if (notacredito.logotipo != null)
                                column.Item().Width(230).Height(70).Image(notacredito.logotipo);
                            else
                                column.Item().Width(230).Height(70).Background(Colors.Grey.Lighten3)
                                .AlignCenter().Text("SIN LOGOTIPO").FontSize(10).FontColor(Colors.Cyan.Accent2);

                            column.Item().PaddingVertical(2).Text(t => {
                                t.Span("Ruc: ").Bold().FontSize(9.5f);
                                t.Span(notacredito.Ruc).Bold().FontSize(9.5f);
                            });

                            column.Item().PaddingTop(4).Text($"{notacredito.RazonSocial}").Bold().FontSize(13f).AlignLeft();

                            column.Item().PaddingVertical(2).Text(t => {
                                t.Span("Dirección Matriz: ").Bold().FontSize(8.5f);
                                t.Span(infoTributaria.DirMatriz).FontSize(8.5f);
                            });

                            column.Item().PaddingVertical(2).Text(t => {
                                t.Span("Direccion Local: ").Bold().FontSize(8.5f);
                                t.Span(notacredito.DireccionLocal).FontSize(8.5f);
                            });
                            column.Item().PaddingVertical(2).Text(t =>
                            {
                                t.Span("Obligado a llevar contabilidad: ").Bold().FontSize(8.5f);
                                t.Span(notacredito.ObligadoContabilidad).FontSize(8.5f);
                            });

                            if (infoTributaria.ContribuyenteRimpe != null)
                                column.Item().PaddingVertical(2).Text(infoTributaria.ContribuyenteRimpe).FontSize(8.5f).Bold();
                            else
                                column.Item().PaddingVertical(2).Text("CONTRIBUYENTE RÉGIMEN RIMPE").FontSize(8.5f).Bold();
                        });

                        row.ConstantItem(20);

                        row.RelativeItem(1.1f).Border(1).BorderColor(Colors.Grey.Darken1).Padding(8).Column(column =>
                        {
                            column.Item().Text("COMPROBANTE DE RETENCIÓN").Bold().FontSize(14).AlignCenter();
                            column.Item().PaddingTop(2).Text(t => {
                                t.Span("No: ").Bold().FontSize(10);
                                t.Span(numeroFactura).Bold().FontColor(Colors.Red.Medium).FontSize(10);
                            });

                            column.Item().PaddingTop(4).Text("Numero Autorización:").Bold().FontSize(8.5f);
                            column.Item().Text(notacredito.ClaveAcceso).FontSize(8f);

                            column.Item().PaddingTop(2).Text(t => {
                                t.Span("Fecha Autorización:").Bold().FontSize(8.5f);
                                t.Span(notacredito.FechaAutorizacion).FontSize(8.5f);
                            });

                            column.Item().PaddingTop(2).Text(t => {
                                t.Span("Ambiente: ").Bold();
                                t.Span(notacredito.Ambiente).FontSize(8.5f);
                            });
                            column.Item().Text(t => {
                                t.Span("Emisión: ").Bold().FontSize(8.5f);
                                t.Span("Normal").FontSize(8.5f);
                            });
                            column.Item().Text("Clave Acceso").Bold().FontSize(8.5f);

                            // Código de barras nativo vectorial e idéntico al de la imagen
                            if (!string.IsNullOrEmpty(notacredito.ClaveAcceso) && notacredito.ClaveAcceso.Length == 49)
                            {
                                column.Item().PaddingTop(8).Height(35).AlignCenter().BarcodeCode128(notacredito.ClaveAcceso);
                            }
                        });
                    });

                    // ================================================================
                    // 2. DATOS DEL CLIENTE (Encerrado entre dos líneas horizontales)
                    // ================================================================
                    page.Content().Column(mainCol =>
                    {
                        mainCol.Item()
                        .BorderTop(0.5f).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1)
                        .PaddingVertical(6).Row(row =>
                        {
                            row.RelativeItem(1.3f).Column(c =>
                            {
                                c.Item().Text(t => { t.Span("Razón social/Nombres Apellidos: ").Bold(); t.Span(notacredito.Cliente); });
                                c.Item().PaddingTop(2).Text(t => { t.Span("Fecha de emisión: ").Bold(); t.Span(notacredito.Emision); });
                                c.Item().PaddingTop(2).Text(t => { t.Span("Dirección: ").Bold(); t.Span(notacredito.DireccionLocal ?? "S/D"); });
                            });
                            row.RelativeItem(0.7f).Column(c =>
                            {
                                c.Item().Text(t => { t.Span("Identificación: ").Bold(); t.Span(notacredito.ClienteNid); });
                            });
                        });

                        mainCol.Item().PaddingTop(15);

                        // ===============================================
                        // 3. CUERPO DEL COMPROBANTE (TABLA + TOTALES)
                        // ===============================================
                        mainCol.Item().Table(table => {
                            // Definición de las columnas
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1.0f); // Código
                                columns.RelativeColumn(1.0f); // Cantidad
                                columns.RelativeColumn(4.7f); // Descripción
                                columns.RelativeColumn(1.5f); // Precio Unitario
                                columns.RelativeColumn(1.3f); // Descuento
                                columns.RelativeColumn(1.5f); // Precio Total
                            });

                            // Definición de la cabeccera
                            table.Header(header =>
                            {
                                var estilo = TextStyle.Default.Bold().FontSize(9.5f);
                                header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Darken1).PaddingBottom(4).Text("Código").Style(estilo);
                                header.Cell().PaddingHorizontal(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).PaddingBottom(4).Text("Cantidad").Style(estilo).AlignRight();
                                header.Cell().PaddingHorizontal(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).PaddingBottom(4).Text("Descripción").Style(estilo);
                                header.Cell().PaddingHorizontal(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).PaddingBottom(4).Text("Precio Unitario").Style(estilo).AlignRight();
                                header.Cell().PaddingHorizontal(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).PaddingBottom(4).Text("Descuento").Style(estilo).AlignRight();
                                header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Darken1).PaddingBottom(4).Text("Precio Total").Style(estilo).AlignRight();
                            });

                            // Filas de los productos comprados (Ej: Hamburguesa Doble)
                            if (notacredito.NotaCreditoDetalles != null && notacredito.NotaCreditoDetalles.Count > 0)
                            {
                                foreach (var detalle in notacredito.NotaCreditoDetalles)
                                {
                                    string punit = string.Format("{0:0.00}", double.Parse(detalle.precioUnitario));
                                    string ptotal = string.Format("{0:0.00}", double.Parse(detalle.precioTotal));
                                    string pdcto = string.Format("{0:0.00}", double.Parse(detalle.descuento));

                                    table.Cell().PaddingVertical(5).Text(detalle.codigo);
                                    table.Cell().PaddingHorizontal(4).PaddingVertical(5).Text(detalle.cantidad).AlignRight();
                                    table.Cell().PaddingHorizontal(4).PaddingVertical(5).Text(detalle.descripcion);
                                    table.Cell().PaddingHorizontal(4).PaddingVertical(5).Text(string.IsNullOrEmpty(punit) ? "0.00" : punit).AlignRight();
                                    table.Cell().PaddingHorizontal(4).PaddingVertical(5).Text(string.IsNullOrEmpty(pdcto) ? "0.00" : pdcto).AlignRight();
                                    table.Cell().PaddingVertical(5).Text(string.IsNullOrEmpty(ptotal) ? "0.00" : ptotal).AlignRight();
                                }
                            }
                            else
                            {
                                foreach (var detalle in documento.Detalles.detalle)
                                {
                                    table.Cell().PaddingVertical(5).Text(detalle.CodigoInterno);
                                    table.Cell().PaddingHorizontal(4).PaddingVertical(5).Text(detalle.Cantidad).AlignRight();
                                    table.Cell().PaddingHorizontal(4).PaddingVertical(5).Text(detalle.Descripcion);
                                    table.Cell().PaddingHorizontal(4).PaddingVertical(5).Text(detalle.PrecioUnitario == "0" ? "0.00" : detalle.PrecioUnitario).AlignRight();
                                    table.Cell().PaddingHorizontal(4).PaddingVertical(5).Text(detalle.Descuento == "0" ? "0.00" : detalle.Descuento).AlignRight();
                                    table.Cell().PaddingVertical(5).Text(detalle.PrecioTotalSinImpuesto == "0" ? "0.00" : detalle.PrecioTotalSinImpuesto).AlignRight();
                                }
                            }

                            table.Cell().ColumnSpan(6).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(0).ExtendHorizontal();

                            // BLOQUE INFERIOR IZQUIERDO: Información Adicional (Sin recuadros pesados)
                            table.Cell().RowSpan(9).ColumnSpan(3).PaddingTop(12).PaddingRight(20).Column(col =>
                            {
                                col.Item().Text("Información Adicional").Bold().FontSize(9.5f);
                                if (documento.InfoAdicional != null)
                                {
                                    foreach (var info in documento.InfoAdicional.CampoAdicional)
                                    {
                                        col.Item().PaddingTop(2).Text(txt => {
                                            txt.Span($"{info.Nombre} : ").Bold().FontSize(8.5f);
                                            txt.Span(info.Text).FontSize(8.5f);
                                        });
                                    }
                                }
                            });

                            var porcentajeIva = infoNotaCredito.TotalConImpuestos.TotalImpuesto[0].DecoPorcentajeImpuesto();

                            var subSinIVA = documento.Detalles.detalle
                                .Where(x => x.Impuestos.Impuesto.Sum(o => double.Parse(o.Valor)) == 0)
                                .Sum(o => double.Parse(o.PrecioTotalSinImpuesto));

                            var subTotal = documento.Detalles.detalle
                                .Where(x => x.Impuestos.Impuesto.Sum(o => double.Parse(o.Valor)) > 0)
                                .Sum(o => double.Parse(o.PrecioTotalSinImpuesto));

                            var descuento = documento.Detalles.detalle
                                .Sum(o => double.Parse(o.Descuento));

                            var total = double.Parse(infoNotaCredito.ValorModificacion);

                            // BLOQUE INFERIOR DERECHO: Tabla de Acumuladores Estilo SRI
                            table.Cell().RowSpan(9).ColumnSpan(3).PaddingTop(12).AlignRight().Table(totalesTable =>
                            {
                                // Definimos la estructura interna de 2 columnas para controlar el ancho de los totales
                                totalesTable.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();   // Columna para las descripciones (Texto alineado a la izquierda por defecto)
                                    columns.ConstantColumn(70); // Columna fija para los valores numéricos alineados a la derecha
                                });

                                var borderGrisClaro = 0.5f;
                                var colorLinea = Colors.Grey.Lighten2;

                                // 1. Subtotal Con Iva
                                totalesTable.Cell()
                                    .BorderBottom(borderGrisClaro).BorderColor(colorLinea)
                                    .PaddingVertical(4).Text($"Subtotal {porcentajeIva}%").FontSize(8.5f);
                                totalesTable.Cell()
                                    .BorderBottom(borderGrisClaro).BorderColor(colorLinea)
                                    .PaddingVertical(4).Text(string.Format("{0:0.00}", subTotal)).AlignRight().FontSize(8.5f);

                                // 2. Subtotal 0%
                                totalesTable.Cell()
                                    .BorderBottom(borderGrisClaro).BorderColor(colorLinea)
                                    .PaddingVertical(4).Text("Subtotal 0%").FontSize(8.5f);
                                totalesTable.Cell()
                                    .BorderBottom(borderGrisClaro).BorderColor(colorLinea)
                                    .PaddingVertical(4).Text(string.Format("{0:0.00}", subSinIVA)).AlignRight().FontSize(8.5f);

                                // 3. Exento de IVA (Corregido "Excento")
                                totalesTable.Cell()
                                    .BorderBottom(borderGrisClaro).BorderColor(colorLinea)
                                    .PaddingVertical(4).Text("Subtotal Exento de IVA").FontSize(8.5f);
                                totalesTable.Cell()
                                    .BorderBottom(borderGrisClaro).BorderColor(colorLinea)
                                    .PaddingVertical(4).Text("0.00").AlignRight().FontSize(8.5f);

                                // 4. Total descuento
                                totalesTable.Cell()
                                    .BorderBottom(borderGrisClaro).BorderColor(colorLinea)
                                    .PaddingVertical(4).Text("Total descuento").FontSize(8.5f);
                                totalesTable.Cell()
                                    .BorderBottom(borderGrisClaro).BorderColor(colorLinea)
                                    .PaddingVertical(4).Text(string.Format("{0:0.00}", descuento)).AlignRight().FontSize(8.5f);

                                // 5. ICE
                                totalesTable.Cell()
                                    .BorderBottom(borderGrisClaro).BorderColor(colorLinea)
                                    .PaddingVertical(4).Text("ICE").FontSize(8.5f);
                                totalesTable.Cell()
                                    .BorderBottom(borderGrisClaro).BorderColor(colorLinea)
                                    .PaddingVertical(4).Text("0.00").AlignRight().FontSize(8.5f);

                                // 6. Propina
                                totalesTable.Cell()
                                    .BorderBottom(borderGrisClaro).BorderColor(colorLinea)
                                    .PaddingVertical(4).Text("Propina").FontSize(8.5f);
                                totalesTable.Cell()
                                    .BorderBottom(borderGrisClaro).BorderColor(colorLinea)
                                    .PaddingVertical(4).Text("0.00").AlignRight().FontSize(8.5f);

                                // 7. VALOR TOTAL
                                totalesTable.Cell()
                                    .BorderBottom(borderGrisClaro).BorderColor(colorLinea)
                                    .PaddingTop(6).PaddingBottom(2).Text("VALOR TOTAL").Bold().FontSize(9f);
                                totalesTable.Cell()
                                    .BorderBottom(borderGrisClaro).BorderColor(colorLinea)
                                    .PaddingTop(6).PaddingBottom(2).Text(string.Format("{0:0.00}", total)).AlignRight().Bold().FontSize(9f);
                            });
                        });
                    });
                });
            });

            if (pdfDocument == null)
            {
                throw new InvalidOperationException("No se pudo inicializar la estructura del documento PDF.");
            }

            using (var stream = new System.IO.MemoryStream())
            {
                pdfDocument.GeneratePdf(stream);
                string path = generaPathArchivo(infoTributaria.NombreComercial, notacredito.ClaveAcceso, "NotaCredito");
                File.WriteAllBytes(path, stream.ToArray());
                return stream.ToArray();
            }

            throw new NotImplementedException();
        }

        private string removeTildesEspacios(string texto)
        {
            texto = texto.Replace("á", "a");
            texto = texto.Replace("é", "e");
            texto = texto.Replace("í", "i");
            texto = texto.Replace("ó", "o");
            texto = texto.Replace("ú", "u");

            texto = texto.Replace("Á", "A");
            texto = texto.Replace("É", "E");
            texto = texto.Replace("Í", "I");
            texto = texto.Replace("Ó", "O");
            texto = texto.Replace("Ú", "U");

            texto = texto.Replace("ñ", "n");
            texto = texto.Replace("Ñ", "N");
            texto = texto.Replace(" ", "");
            texto = texto.Trim();

            return texto;
        }

        private string generaPathArchivo(string empresa, string claveAcceso, string tipo)
        {
            string baseDirectory = Path.Combine(AppContext.BaseDirectory, "PDFs");
            var fecha = DateTime.Now;
            var anio = fecha.Year;
            var mes = fecha.Month;

            string carpetaDestino = Path.Combine(
                baseDirectory,
                removeTildesEspacios(empresa),
                tipo,
                anio.ToString(),
                mes.ToString("00")
            );

            if (!Directory.Exists(carpetaDestino))
            {
                Directory.CreateDirectory(carpetaDestino);
            }

            string nombreArchivo = $"{claveAcceso}.pdf";
            string path = Path.Combine(carpetaDestino, nombreArchivo);
            return path;
        }

        private string getFormaPago(string codigo)
        {
            return codigo switch
            {
                "01" => "Sin utilización del Sistema Financiero",
                "15" => "Compensación de deudas",
                "16" => "Tarjeta de Débito",
                "17" => "Dinero Electrónico",
                "18" => "Tarjeta Prepago",
                "19" => "Tarjeta de Crédito",
                "20" => "Otros con utilización del Sistema Financiero",
                "21" => "Endoso de Títulos",
                _ => string.Empty
            };
        }
    }
}
