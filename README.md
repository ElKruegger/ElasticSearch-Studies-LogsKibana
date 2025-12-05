# ElasticStudiesLogsKibana

## 📚 Sobre o Projeto

Este é um projeto **simples de estudos** focado em demonstrar a integração de uma API REST desenvolvida em **C# (.NET 8.0)** com **Elasticsearch** e **Kibana** para monitoramento e análise de logs estruturados.

O projeto foi criado com o objetivo de aprender e praticar conceitos de:
- Logging estruturado com Serilog
- Integração com Elasticsearch
- Visualização de logs no Kibana
- Boas práticas de logging em APIs REST

## 🎯 Objetivo

O objetivo principal deste projeto é servir como um **laboratório de estudos** para entender como:
1. Implementar logging estruturado em uma API .NET usando Serilog
2. Enviar logs automaticamente para o Elasticsearch
3. Visualizar e analisar logs no Kibana
4. Aplicar boas práticas de logging em operações CRUD

## 🔧 O que o Projeto Faz

O projeto consiste em uma **API REST simples** que oferece:

### Funcionalidades

1. **API de Produtos (CRUD completo)**
   - `POST /api/product` - Criar produto
   - `GET /api/product` - Listar produtos (com filtros opcionais por categoria e marca)
   - `GET /api/product/{id}` - Buscar produto por ID
   - `PUT /api/product/{id}` - Atualizar produto
   - `DELETE /api/product/{id}` - Excluir produto
   - `GET /api/product/stats` - Obter estatísticas dos produtos

2. **API de Logs de Teste**
   - `GET /log/generate` - Gera logs de exemplo (Information, Warning, Error) para testar a integração com Elasticsearch

### Sistema de Logging

Todos os endpoints geram logs estruturados que são enviados para:
- **Console** (para visualização imediata durante desenvolvimento)
- **Elasticsearch** (para armazenamento e análise)
- **Kibana** (para visualização e dashboards)

Os logs incluem informações como:
- Nível de log (Information, Warning, Error)
- Timestamp
- RequestId (para rastreamento de requisições)
- Detalhes contextuais (IDs de produtos, categorias, valores, etc.)
- Informações de requisições HTTP (método, path, status code, tempo de resposta)

## 🛠️ Como é Feito

### Tecnologias Utilizadas

- **.NET 8.0** - Framework da aplicação
- **ASP.NET Core** - Framework web para criação da API REST
- **Serilog** - Biblioteca de logging estruturado
- **Serilog.Sinks.Elasticsearch** - Sink do Serilog para enviar logs ao Elasticsearch
- **Elasticsearch 8.15.0** - Motor de busca e análise de logs
- **Kibana 8.15.0** - Interface de visualização e análise de logs
- **Docker & Docker Compose** - Para orquestração do Elasticsearch e Kibana
- **Swagger** - Documentação interativa da API

### Arquitetura

```
┌─────────────────┐
│   API .NET 8.0  │
│  (ASP.NET Core) │
└────────┬────────┘
         │
         │ Logs estruturados
         │
    ┌────▼────┐
    │ Serilog │
    └────┬────┘
         │
    ┌────▼──────────┐
    │ Elasticsearch │
    └────┬──────────┘
         │
    ┌────▼────┐
    │ Kibana  │
    └─────────┘
```

### Estrutura do Projeto

```
ElasticStudiesLogsKibana/
├── Controllers/
│   ├── ProductController.cs    # Endpoints CRUD de produtos
│   └── LogController.cs        # Endpoint para gerar logs de teste
├── Models/
│   └── Product.cs              # Modelo de dados do produto
├── Program.cs                   # Configuração da aplicação e Serilog
├── appsettings.json            # Configurações do Serilog e Elasticsearch
└── docker-compose.yml          # Configuração do Elasticsearch e Kibana
```

## 📋 Pré-requisitos

Antes de executar o projeto, certifique-se de ter instalado:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (ou Docker + Docker Compose)

## 🚀 Como Executar

### 1. Iniciar Elasticsearch e Kibana

Primeiro, inicie os serviços do Elasticsearch e Kibana usando Docker Compose:

```bash
docker-compose up -d
```

Isso irá iniciar:
- **Elasticsearch** na porta `9200`
- **Kibana** na porta `5601`

Aguarde alguns segundos para os serviços iniciarem completamente. Você pode verificar se o Elasticsearch está rodando acessando: `http://localhost:9200`

### 2. Executar a API

Navegue até a pasta do projeto e execute:

```bash
cd ElasticStudiesLogsKibana
dotnet restore
dotnet run
```

A API estará disponível em:
- **API**: `http://localhost:5000` ou `https://localhost:5001`
- **Swagger UI**: `http://localhost:5000/swagger` (em modo Development)

### 3. Acessar o Kibana

1. Abra seu navegador e acesse: `http://localhost:5601`
2. No Kibana, vá em **Stack Management** > **Index Patterns**
3. Crie um index pattern: `csharp-api-logs-*`
4. Selecione o campo de timestamp: `@timestamp`
5. Agora você pode visualizar os logs em **Discover**

### 4. Testar a API

#### Gerar Logs de Teste

```bash
GET http://localhost:5000/log/generate
```

#### Criar um Produto

```bash
POST http://localhost:5000/api/product
Content-Type: application/json

{
  "name": "Camiseta Básica",
  "description": "Camiseta de algodão 100%",
  "category": "Roupas",
  "brand": "Marca X",
  "price": 49.90,
  "stockQuantity": 50,
  "size": "M",
  "color": "Branco"
}
```

#### Listar Produtos

```bash
GET http://localhost:5000/api/product
```

#### Buscar Produto por ID

```bash
GET http://localhost:5000/api/product/{id}
```

#### Atualizar Produto

```bash
PUT http://localhost:5000/api/product/{id}
Content-Type: application/json

{
  "name": "Camiseta Básica Atualizada",
  "price": 39.90,
  "stockQuantity": 30
}
```

#### Excluir Produto

```bash
DELETE http://localhost:5000/api/product/{id}
```

#### Obter Estatísticas

```bash
GET http://localhost:5000/api/product/stats
```

## 📊 Visualizando Logs no Kibana

Após executar algumas requisições na API:

1. Acesse o Kibana: `http://localhost:5601`
2. Vá em **Discover**
3. Selecione o index pattern `csharp-api-logs-*`
4. Você verá todos os logs gerados pela API
5. Use os filtros e campos para analisar os logs:
   - `RequestId` - Para rastrear uma requisição específica
   - `ProductId` - Para ver logs relacionados a um produto
   - `Category`, `Brand` - Para filtrar por categoria ou marca
   - Níveis de log (Information, Warning, Error)

## 🎓 Foco do Projeto

Este projeto é focado em:

1. **Aprendizado Prático**: Entender na prática como funciona a integração entre .NET, Serilog, Elasticsearch e Kibana
2. **Logging Estruturado**: Aprender a criar logs estruturados e contextualizados
3. **Observabilidade**: Demonstrar como logs podem ser usados para monitorar e debugar aplicações
4. **Boas Práticas**: Aplicar boas práticas de logging em APIs REST, incluindo:
   - Rastreamento de requisições com RequestId
   - Logging contextual com informações relevantes
   - Diferentes níveis de log (Information, Warning, Error)
   - Logging de operações CRUD com detalhes relevantes

## 📝 Notas Importantes

- Este é um projeto de **estudos** e não deve ser usado em produção sem as devidas adaptações
- Os dados dos produtos são armazenados em memória (não há persistência em banco de dados)
- O Elasticsearch está configurado sem segurança (`xpack.security.enabled=false`) apenas para facilitar os estudos
- Em produção, é recomendado configurar autenticação e segurança adequadas

## 🛑 Parar os Serviços

Para parar o Elasticsearch e Kibana:

```bash
docker-compose down
```

Para remover também os volumes (dados):

```bash
docker-compose down -v
```

## 📚 Recursos de Aprendizado

- [Documentação do Serilog](https://serilog.net/)
- [Documentação do Elasticsearch](https://www.elastic.co/guide/en/elasticsearch/reference/current/index.html)
- [Documentação do Kibana](https://www.elastic.co/guide/en/kibana/current/index.html)
- [ASP.NET Core Logging](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/)

## 📄 Licença

Este projeto é apenas para fins educacionais e de estudo.

