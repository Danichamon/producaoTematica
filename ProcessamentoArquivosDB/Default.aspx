<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ProcessamentoArquivosDB._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>Importar Arquivo Base Mensal</h1>
        <p class="lead">ASP.NET is a free web framework for building great Web sites and Web applications using HTML, CSS, and JavaScript.</p>
        <p>  <asp:Button runat="server" ID="btSalvar" title="Clique para salvar resultado contestacao"  OnClick="SalvarRegistro" type="submit" Text="Gravar Resultado" class="btn btn-warning btn-md botao-salvar invisivel"></asp:Button>
</p>
    </div>

    
   

</asp:Content>
