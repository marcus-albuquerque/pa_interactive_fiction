extends Node

enum Raca {
	ELFO,
	ORC,
	HUMANO,
	ANAO
}

var nome: String = "Jogador"
var raca: Raca = Raca.HUMANO
var sexo: String = "Indefinido"
var idade: int = 20
var descricao: String = "Uma descrição genérica e breve."


var localizacao: String = "inicio"

func _ready():
	pass

# Você pode adicionar outras funções aqui, por exemplo:
# func set_nome(novo_nome):
# 	nome = novo_nome
#
# func mover(nova_localizacao):
# 	localizacao = nova_localizacao
