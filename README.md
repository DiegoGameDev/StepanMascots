# StepanMascot

StepanMascot é um software experimental feito em C# com OpenTK para exibir um mascote animado sobre a tela.

A ideia principal é simples: o programa recebe uma textura/spritesheet, gera os sprites com base em uma regra definida pelo usuário e usa esses sprites para desenhar o mascote em uma janela transparente, sem bordas e sempre acima das outras janelas.

## Status

Este projeto ainda está em desenvolvimento.

No momento, não há release oficial publicada. O repositório contém apenas o código-fonte para estudo, testes e evolução do projeto.

## Funcionalidades

- Carrega uma textura/spritesheet.
- Gera sprites a partir de regras aplicadas pelo usuário.
- Ignora sprites totalmente transparentes.
- Renderiza o mascote usando OpenTK.
- Mantém a janela sempre no topo da tela.
- Usa uma janela sem bordas.
- Permite fundo transparente.
- Só bloqueia visualmente o que está atrás do mascote, mas ainda é possível interagir com o desktop normalmente.
- Pode ser usado como base para mascotes de desktop.

## Objetivo

O objetivo do StepanMascot é permitir a criação de mascotes visuais para desktop, posicionados sobre a tela, sem interferir diretamente no uso normal do computador.

Ele funciona apenas como uma camada visual: o mascote aparece por cima, mas não deve atrapalhar a interação com janelas ou elementos atrás dele.

## Tecnologias

- C#
- .NET
- OpenTK
- OpenGL

## Como funciona

O fluxo básico do programa é:

1. O usuário fornece uma textura ou spritesheet.
2. O programa divide a textura em sprites.
3. Cada sprite é analisado.
4. Sprites que não possuem pixels visíveis são descartados.
5. Os sprites válidos são salvos e usados para renderizar o mascote.
6. O mascote é desenhado em uma janela transparente, sem bordas e always-on-top.

## Instalação

Ainda não há instalador ou release pronta.

Para testar o projeto, clone o repositório e execute o código-fonte usando o .NET SDK.

```bash
git clone https://github.com/seu-usuario/StepanMascot.git
cd StepanMascot
dotnet run
