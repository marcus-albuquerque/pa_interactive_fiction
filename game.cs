using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot.JavaScript;

public partial class RoomData : GodotObject
{
	public string Nome { get; set; }
	public string Descricao { get; set; }
	public Dictionary<string, string> Saidas { get; set; } = new Dictionary<string, string>();
}

public partial class TextAdventure : Control
{
	private string _salaIdAtual = "inicio";
	private bool _javascriptBridgeAvailable = false;
	
	[Export]
	private NodePath _textoPrincipalPath = "VBoxContainer/TextoPrincipal";
	private RichTextLabel _textoPrincipal;

	[Export]
	private NodePath _entradaComandoPath = "VBoxContainer/EntradaComando";
	private LineEdit _entradaComando;

	private readonly Dictionary<string, RoomData> _salas = new Dictionary<string, RoomData>()
	{
		["inicio"] = new RoomData
		{
			Nome = "(1) Enfermaria do Santuário de Tisadya ",
			Descricao = "Seus olhos se abrem lentamente. Você sente o aroma de uma mistura de ervas secas, lavanda e incenso suave. Você está sobre uma cama de palha, não muito macia, coberto por um lençol de linho áspero.\nO cômodo é pequeno e simples e, as paredes, de uma alvenaria irregular. O lugar está silencioso, exceto pelo som rítmico de um pilão no qual se moem ervas e pelo gotejar suave de água em uma bacia de barro no canto.\nA luz é fraca, vinda de algumas velas de sebo e de uma pequena janela próxima ao teto.\nO quarto é uma enfermaria simples; há mais dois catres vazios como o seu. Ao seu lado, há um banco de madeira gasto de três pernas e uma pequena prateleira na parede, onde repousam alguns frascos de barro etiquetados à mão e rolos de ataduras amareladas.\nParando de moer as ervas, a elfa seca as mãos em um pano preso ao cinto. Suas vestes são de lã simples, tingidas de um branco desbotado, práticas e gastas nos cotovelos, mas limpas. Seu rosto é sereno e seus olhos, de um verde profundo, o analisam com uma calma profissional.\nEla pára ao lado da sua cama. Bom. Você acordou. Sua voz é suave, mas firme. Eu sou Éleyren, a Sacerdotisa de Tisadya. Você está na enfermaria do Santuário. Encontraram você caído na floresta a oeste daqui, há dois dias, sem sentidos.\nEla inclina a cabeça, seus olhos fixos nos seus. Eu curei o que pude, mas o resto depende de Tisadya. . . e de você. Você se lembra de como chegou aqui? Se lembra do seu nome?\nVocê não se lembra de nada do seu passado: nação, familia. . . Tudo o que você se lembra é seu nome.\nAo leste você vê uma porta de madeira que leva ao santuário.\n",
			Saidas = new Dictionary<string, string> { ["leste"] = "santuario_tisadya" }
		},
		["santuario_tisadya"] = new RoomData
		{
			Nome = "(2) Santuário de Tisadya ",
			Descricao = "O espaço é modesto e emana uma tranquilidade profunda. O teto é sustentado por vigas de madeira escura e áspera, e o chão é de lajotas de pedra gastas pelo tempo. Bancos de madeira simples, polidos pelo uso, estão alinhados em direção a um altar de pedra elevado. Atrás do altar, dominando o santuário, está a estátua de Tisadya. Pequenas oferendas, como fitas trançadas, flores silvestres e tigelas de grãos, estão dispostas aos pés da estátua, deixadas pelos poucos fiéis da vila.\nAo oeste você vê uma porta de madeira que leva à enfermaria.\nAo norte há um portal ogival que leva ao cemitério.\nAo sul há um portal que leva para o jardim do santuário.\nAo leste há uma porta de madeira que leva aos aposentos de Éleyren\n",
			Saidas = new Dictionary<string, string>
			{
				["norte"] = "cemiterio",
				["sul"] = "jardim_santuario",
				["leste"] = "aposentos_eleyren",
				["oeste"] = "inicio"
			}
		},
		["aposentos_eleyren"] = new RoomData
		{
			Nome = "(3) Aposento de Éleyren",
			Descricao = "Um reflexo da personalidade prática da sacerdotisa, o aposento é pequeno e austero. O cheiro de ervas secas e incenso é mais suave aqui, misturado ao cheiro típico de pergaminho velho.\nA luz é fraca, vinda de uma única vela sobre uma escrivaninha e de uma fresta estreita na parede de pedra, ao norte, que serve como janela. Em um canto há uma cama estreita com uma manta de lã dobrada. Um pequeno baú de madeira, sem adornos, fica aos pés dela. Na parede oposta à porta, um nicho raso foi cavado na pedra, contendo uma versão em miniatura, entalhada em madeira, de Tisadya.\n",
			Saidas = new Dictionary<string, string> { ["oeste"] = "santuario_tisadya" }
		},
		["cemiterio"] = new RoomData
		{
			Nome = "(4) Cemitério ",
			Descricao = "O ar aqui é notavelmente mais frio do que no santuário. O  cemitério é pequeno e murado, tendo seu silêncio quebrado apenas pelo farfalhar do vento e pelo barulho de corvos. Lápides de pedra cinzenta, gastas pelo tempo e cobertas de líquen, pontilham a grama alta e mal cuidada.\nAo fundo, contra o muro norte, ergue-se um mausoléu de pedra, sua porta de ferro pesada e enferrujada.\nNo muro ao leste se vê uma fenda na parte de baixo, suficiente para passar uma pessoa.\n",
			Saidas = new Dictionary<string, string>
			{
				["leste"] = "lugar_escuro",
				["norte"] = "mausoleu_bezeit"
			}
		},
		["lugar_escuro"] = new RoomData
		{
			Nome = "(5) Lugar Escuro",
			Descricao = "Este é um beco estreito e sem saída, espremido entre o alto muro do cemitério a oeste e a parede cega e úmida de um armazém abandonado a leste. Neste pequeno espaço a luz do sol mal penetra. Pilhas de barris quebrados e sacos rasgados estão jogados contra a parede do armazém, claramente servindo de esconderijo para ratos.\nUma única lamparina enferrujada ilumina o local. Em pé, sob a sua luz, vê-se um homem esguio com roupas escuras. É Toyol. Ele está de braços cruzados, encostado na parede, afiando pacientemente uma adaga.\nNo muro ao oeste se vê a fenda que dá para o cemitério.\n",
			Saidas = new Dictionary<string, string> { ["oeste"] = "cemiterio" }
		},
		["mausoleu_bezeit"] = new RoomData
		{
			Nome = "(6) Mausoléu do Patriarca Bezeit ",
			Descricao = "Você chega a uma pequena cascata escondida, onde a água desce sobre rochas cobertas de musgo. Há um nicho sombrio atrás da água. A trilha continua, subindo íngreme, a oeste.",
			Saidas = new Dictionary<string, string>
			{
				["sul"] = "cemiterio",
				["oeste"] = "catacumbas_secretas"
			}
		},
		["catacumbas_secretas"] = new RoomData
		{
			Nome = "Catacumbas Secretas",
			Descricao = "As catacumbas existentes sob a Vila Boeka. Para voltar leste",
			Saidas = new Dictionary<string, string> { ["leste"] = "mausoleu_bezeit" }
		},
		["jardim_santuario"] = new RoomData
		{
			Nome = "(8) Jardim do Santuário ",
			Descricao = "Este é um pequeno pátio murado que serve como entrada principal para o santuário. O espaço é dominado por vários canteiros de madeira elevados, onde crescem de forma organizada dezenas de ervas e plantas medicinais. Contra o muro oeste há um banco de pedra simples para descanso.\nAo norte, um portal ogival leva para dentro do Santuário de Tisadya.\nAo sul, um portão de madeira simples, mas robusto, leva à praça central da vila.\n",
			Saidas = new Dictionary<string, string>
			{
				["norte"] = "santuario_tisadya",
				["sul"] = "praca_central"
			}
		},
		["praca_central"] = new RoomData
		{
			Nome = "(9) Praça Central ",
			Descricao = "Este é o coração da vila Boeka, um espaço aberto de terra batida. O ar vibra com o som de vozes, o ranger ocasional de uma carroça e o tilintar distante do martelo de um ferreiro. No centro exato da praça há um poço de pedra, sua borda gasta e entalhada pelo atrito de inúmeras cordas.\nAo norte, você vê o portão de madeira simples que leva ao Jardim do Templo.\nA saída leste da rua principal leva em direção ao som do martelar de um ferreiro.\nA saída oeste da rua principal leva em direção ao que parece uma loja de alquimia.\nAo sul da praça é possível ver um edifício de madeira: a taverna Javali Confuso.\n",
			Saidas = new Dictionary<string, string>
			{
				["norte"] = "jardim_santuario",
				["sul"] = "calice_sagrado",
				["leste"] = "rua_principal_13",
				["oeste"] = "rua_principal_14"
			}
		},
		["calice_sagrado"] = new RoomData
		{
			Nome = "(10) Taverna Cálice Sagrado",
			Descricao = "O ar quente e enfumaçado da taverna atinge você, carregado com o cheiro de hidromel derramada, gordura de porco e fumaça de cachimbo. É um salão amplo, mas de teto baixo, sustentado por vigas de madeira escura. O som de conversas altas, canecas batendo na madeira e uma música desafinada de alaúde vinda de um bardo sonolento no canto enchem o lugar.\nMesas de madeira pesada, manchadas e arranhadas, estão espalhadas pelo chão coberto de serragem, ocupadas por alguns moradores. No fundo de um balcão de bar, Befur, o taverneiro, limpa uma caneca com um pano sujo.\nUma lareira crepitante na parede leste aquece o ambiente e, acima dela, é possível ver a cabeça empalhada de um javali com uma expressão estranhamente perplexa.\nA porta principal ao norte leva de volta à praça central.\nHá um pequeno alçapão no chão, atrás do balcão.\nHá uma escada de madeira subindo pela lateral, ao lado do balcão.\n",
			Saidas = new Dictionary<string, string>
			{
				["norte"] = "praca_central",
				["cima"] = "estalagem",
				["descer"] = "estoque_taverna"
			}
		},
		["estoque_taverna"] = new RoomData
		{
			Nome = "(11) Estoque da Taverna ",
			Descricao = "O cômodo é pequeno, baixo e desorganizado. O chão de pedra está grudento em alguns pontos. Estantes de madeira tortas se alinham nas paredes leste e oeste, mas estão em boa parte vazias, exceto por alguns sacos de grãos rasgados e garrafas de vinho cobertas de poeira. Várias caixas soltas estão empilhadas aleatoriamente no centro, cobertas por grossas teias de aranha. Em um canto nos fundos, parcialmente escondido por uma pilha de barris vazios, há um canto de dormir improvisado. Uma única lâmpada a óleo, pendurada no teto baixo, mal ilumina o lugar.\nAcima está o alçapão que trouxe você aqui, leva você de volta ao salão da taverna.\n",
			Saidas = new Dictionary<string, string> { ["subir"] = "calice_sagrado" }
		},
		["estalagem"] = new RoomData
		{
			Nome = "(12) Aposento para pernoitar ",
			Descricao = "Aqui os sons da taverna abaixo são ouvidos apenas como um murmúrio baixo e o tilintar ocasional de canecas batendo. Uma cama estreita com um colchão de palha irregular e cobertores de lã áspera ocupa a maior parte do espaço. Ao lado há uma pequena cômoda de madeira com uma bacia de metal lascada e uma jarra de água. Uma única janela pequena e empoeirada na parede norte deixa entrar uma luz cinzenta.\nA porta ao sul leva de volta à escada que leva ao salão da taverna.\n",
			Saidas = new Dictionary<string, string> { ["descer"] = "calice_sagrado" }
		},
		["rua_principal_13"] = new RoomData
		{
			Nome = "(13) Rua Principal",
			Descricao = "Esta é a principal artéria de comércio da vila, embora seja pouco mais que um caminho largo e empoeirado. A rua é dominada pelo barulho e calor que emanam de um dos edifícios.\nAo norte, você vê a fonte do barulho: uma forja de frente aberta, de onde sai o brilho de um forno e o clang de metal contra metal.\nAo sul, diretamente oposta à forja, há um edifício robusto de madeira com uma placa de um elmo e um peitoral pintados de forma grosseira: uma loja de armaduras.\nA rua continua mais para leste.\nA Praça Central fica a oeste.\n",
			Saidas = new Dictionary<string, string>
			{
				["norte"] = "forja_thovak",
				["sul"] = "loja_armadura",
				["leste"] = "portao_leste_19",
				["oeste"] = "praca_central"
			}
		},
		["forja_thovak"] = new RoomData
		{
			Nome = "(16) A Forja de Thorvak",
			Descricao = "A forja é um espaço funcional e caótico. A bigorna maciça domina o centro. No fundo (sul), a fornalha queima com um brilho laranja-avermelhado, irradiando um calor intenso. As paredes estão cobertas de ferramentas penduradas: martelos, espadas, flechas. . . Prateleiras e barris estão cheios de peças de metal, ferraduras, pregos e uma diversidade de armas em diferentes estágios de conclusão ou reparo. Thorvak, o ferreiro anão, está parado ao lado da bigorna, limpando o suor com um antebraço musculoso.\nA saída é ao ao sul, que leva de volta à Rua Principal.\nExiste uma escada que leva para cima",
			Saidas = new Dictionary<string, string>
			{
				["subir"] = "aposento_thovak",
				["sul"] = "rua_principal_13"
			}
		},
		["aposento_thovak"] = new RoomData
		{
			Nome = "(17) Aposento de Thorvak",
			Descricao = "Este pequeno aposento é um reflexo direto da personalidade de Thorvak, funcional e sem frescuras. As paredes de pedra bruta são escuras de fuligem, e o chão é de lajota nua, varrido de forma grosseira. Em um canto há uma cama baixa e larga, feita de tábuas grossas de madeira, coberta por uma pilha de peles desgrenhadas. Ao lado, um caixote virado serve de mesa, sobre o qual repousa uma caneca de metal amassada e um prato de estanho com migalhas. Aos pés da cama há um baú pesado, feito de madeira escura e reforçado com tiras grossas de ferro. Encostado na parede ao lado da cama, há um suporte vazio, feito para sustentar, ao que tudo indica, um grande machado.\nA única saída é a porta ao sul, que leva de volta à Forja de Thorvak",
			Saidas = new Dictionary<string, string> { ["descer"] = "forja_thovak" }
		},
		["loja_armadura"] = new RoomData
		{
			Nome = "(18) Loja de Armaduras de Gor-Noj",
			Descricao = "O lugar é relativamente bem cuidado. É possível ouvir o barulho abafado da forja de Thorvak, do outro lado da rua, reduzido a um clang rítmico e distante. O cheiro dominante é uma mistura forte de couro curtido, óleo de polimento e cera de abelha. O espaço é amplo, mas parece abarrotado devido à enorme quantidade de equipamentos pendurados. Peças de armadura estão por toda parte: peitorais de couro batido pendurados em vigas do teto, cotas de malha enroladas em prateleiras na parede leste, e uma fileira de elmos variados (alguns amassados e reparados) em uma longa mesa central. Vários escudos de madeira e metal estão encostados na parede oeste. O dono, Gor-Noj, um meio-orc corpulento, está atrás de um balcão de madeira rústico no fundo da loja. Ele está metodicamente passando um pano oleado em um escudo de metal, ignorando sua entrada por um momento antes de levantar o olhar lentamente.\nA porta ao norte leva de volta à Rua Principal.",
			Saidas = new Dictionary<string, string> { ["norte"] = "rua_principal_13" }
		},
		["portao_leste_19"] = new RoomData
		{
			Nome = "(19) Portão Leste",
			Descricao = "Aqui termina a rua principal, encontrando a alta paliçada de madeira que protege a vila. O Portão Leste é uma estrutura robusta de troncos grossos, reforçada com faixas de ferro e equipada com uma pesada tranca de madeira que atualmente repousa em seus suportes. O guarda humano, Beder, está postado no lado norte do portão; a guarda anã, Nirja, está postada no lado sul. Acima deles, uma plataforma de vigia permite observar o exterior. O portão está atualmente entreaberto, permitindo a passagem de uma pessoa de cada vez. Através dele, você pode ver um caminho de terra batida que persiste pelos campos ao leste.\nA rua principal se estende para oeste, de volta ao coração da vila.\nO caminho de terra batida a leste continua para fora da vila.",
			Saidas = new Dictionary<string, string>
			{
				["leste"] = "caminho_para_mina",
				["oeste"] = "rua_principal_13"
			}
		},
		["caminho_para_mina"] = new RoomData
		{
			Nome = "Caminho para a Mina**",
			Descricao = "Um pequeno trecho que leva para as minas abandonadas de Boeka. Você pode voltar pelo portao da vila a oeste, ou entrar na mina ao norte",
			Saidas = new Dictionary<string, string>
			{
				["norte"] = "mina_abandonada",
				["oeste"] = "portao_leste_19"
			}
		},
		["mina_abandonada"] = new RoomData
		{
			Nome = "Mina Abandonada**",
			Descricao = "Apenas seguindo o mapa, Corredores superiores diga subir, ou descer para corredores inferiores. A saida está ao sul",
			Saidas = new Dictionary<string, string>
			{
				["subir"] = "corredores_superiores_mina",
				["descer"] = "corredores_inferiores_mina",
				["sul"] = "caminho_para_mina"
			}
		},
		["corredores_superiores_mina"] = new RoomData
		{
			Nome = "Corredores Superiores",
			Descricao = "Onde os monstros residem, podemos fazer um pequeno labirinto. Para voltar é só descer",
			Saidas = new Dictionary<string, string> { ["descer"] = "mina_abandonada" }
		},
		["corredores_inferiores_mina"] = new RoomData
		{
			Nome = "Corredores Inferiores",
			Descricao = "Onde os monstros residem, podemos por um item. Para voltar é só subir",
			Saidas = new Dictionary<string, string> { ["subir"] = "mina_abandonada" }
		},
		["rua_principal_14"] = new RoomData
		{
			Nome = "(14) Rua Principal",
			Descricao = "Esta seção larga da Rua Principal serve como uma área de logística e patrulha. Ao norte fica um edifício de madeira robusto, de aparência funcional, com uma bandeira simples da vila pendurada na frente: o Posto da Milícia. Ao sul, há apenas mato alto com o que parecem ruínas de um armazém, e um caminho estreito seguindo em direção ao Portão Sul.\nA praça central fica a leste.\nA rua continua para oeste, levando à seção final da rua principal.\nAo sul um caminho mais estreito corre em direção ao Portão Sul.\nAo norte há uma porta que leva ao Posto da Milícia.\n",
			Saidas = new Dictionary<string, string>
			{
				["norte"] = "posto_milicia",
				["sul"] = "portao_sul",
				["leste"] = "praca_central",
				["oeste"] = "rua_principal_15"
			}
		},
		["portao_sul"] = new RoomData
		{
			Nome = "(20) Portão Sul",
			Descricao = "Esta é a barreira sul da vila Boeka. O Portão Sul é uma estrutura imponente de madeira escura e ferro, parecendo mais robusto e permanentemente fechado do que o Portão Leste. O cheiro de piche, usado para tratar a madeira, e um leve odor de mofo pairam no ar. Elnor, um arqueiro elfo-humano nervoso, está sobre a vigia. Kel, uma maga humana, está à direita, o olhar fixo no caminho além do portão. Uma plataforma de vigia ergue-se acima da paliçada. O portão está fechado.\nAtravés das frestas e da torre de vigia, você pode ver a estrada sul, um caminho que rapidamente se torna sombrio, flanqueado por árvores retorcidas e uma névoa rasteira que parece pairar mesmo sob o sol.\nAo norte, o caminho leva de volta à Rua Principal.\n",
			Saidas = new Dictionary<string, string> { ["norte"] = "rua_principal_14" }
		},
		["posto_milicia"] = new RoomData
		{
			Nome = "(21) Posto da Milícia",
			Descricao = "Este é o quartel-general da milícia de Boeka. É um salão único, rústico e funcional, com paredes de troncos grossos e um teto baixo sustentado por vigas escuras. Um mapa grande da região está estendido sobre uma mesa central, marcado com círculos vermelhos e anotações. Pelo salão, há racks de armas contendo lanças e espadas curtas, e alguns escudos com o brasão simples da vila estão encostados na parede. O Capitão Jaffer está em uma mesa no canto oeste, observando o movimento.\nUma porta de madeira simples a oeste parece levar a um escritório particular.\nUma arcada larga a leste leva a uma área com piso de terra batida, de onde vêm sons de esforço físico.\nUma porta ao norte, entreaberta, parece ser um local para a guarda dormir.\nNo canto sudeste, um alçapão de madeira pesada com um anel de ferro dá acesso ao subsolo. A porta principal ao sul leva de volta à Rua Principal.\n",
			Saidas = new Dictionary<string, string>
			{
				["norte"] = "aposento_guarda",
				["sul"] = "rua_principal_14",
				["leste"] = "treinamento_milicia",
				["oeste"] = "aposento_jaffer",
				["descer"] = "prisao_milicia"
			}
		},
		["aposento_jaffer"] = new RoomData
		{
			Nome = "(22) Aposento de Jaffer",
			Descricao = "Este quarto é espartano, contendo apenas o essencial. As paredes de troncos são nuas. Uma cama de campanha estreita está encostada na parede norte, coberta por um cobertor de lã cinza, dobrado com precisão militar. Aos pés da cama, há um baú pessoal de madeira escura, reforçado com metal. Na parede oeste, há uma pequena janela gradeada e um suporte de armas vazio. Em um canto, há uma pequena bacia de estanho e uma jarra de água sobre um suporte de madeira.\nA única porta, a leste, leva de volta ao Posto da Milícia.",
			Saidas = new Dictionary<string, string> { ["leste"] = "posto_milicia" }
		},
		["aposento_guarda"] = new RoomData
		{
			Nome = "(23) Aposento da Guarda",
			Descricao = "O ambiente aqui é quente e abafado, com um cheiro forte de suor, roupa suja e couro velho. Este é um dormitório simples e abarrotado. Seis beliches de madeira bruta estão alinhados contra as paredes leste e oeste, a maioria deles desarrumada, com cobertores de lã jogados de qualquer maneira. Pequenos baús pessoais de madeira estão guardados sob cada beliche. No centro do cômodo, uma mesa tosca está cercada por alguns bancos, coberta de canecas de cerveja vazias, um baralho de cartas gorduroso e migalhas de uma refeição passada. Roupas sujas e peças de armadura de couro estão jogadas sobre os bancos e no chão.\nA única porta, ao sul, leva de volta ao Posto da Milícia.",
			Saidas = new Dictionary<string, string> { ["sul"] = "posto_milicia" }
		},
		["treinamento_milicia"] = new RoomData
		{
			Nome = "(24) Sala de Treinamento",
			Descricao = "Esta área é um pátio de armas funcional, com o chão de terra batida e compactada. O teto é alto e parcialmente aberto às vigas, permitindo a ventilação. Ao longo da parede leste há uma fileira de três bonecos de treino, feitos de palha amarrada em postes de madeira grossa. Seus corpos estão esfarrapados de tantos golpes. Um rack de armas de madeira na parede norte contém várias espadas de madeira cegas, bastões longos e alguns escudos de prática, lascados.\nA arcada a oeste leva de volta ao salão principal do Posto da Milícia.\n",
			Saidas = new Dictionary<string, string> { ["oeste"] = "posto_milicia" }
		},
		["prisao_milicia"] = new RoomData
		{
			Nome = "(25) Prisão",
			Descricao = "O ar aqui embaixo é frio, parado e fétido. O teto de pedra baixo goteja água suja, formando poças no chão de terra irregular. A única luz vem de uma única lamparina em um suporte na parede perto da escada, lançando sombras longas e distorcidas. Este porão serve como a prisão da vila. Há duas celas pequenas, mas robustas, de barras de ferro enferrujadas, ocupando a parede norte. A maior parte do espaço restante é ocupada por barris velhos e caixas quebradas, cobertos de teias de aranha. Em um canto, perto da escada, há uma mesa tosca e um banco, presumivelmente para um guarda, embora esteja vazio agora.\nA única saída é a escada de pedra que leva de volta ao Posto da Milícia.",
			Saidas = new Dictionary<string, string> { ["subir"] = "posto_milicia" }
		},
		["rua_principal_15"] = new RoomData
		{
			Nome = "(15) Rua Principal",
			Descricao = "Esta é a seção final da rua principal dentro das paliçadas. É um caminho de terra batida como o resto, mas as laterais são limpas. Ao sul há um edifício singular. É uma pequena loja de pedra e madeira, com uma placa pendurada que mostra um pilão e uma retorta borbulhante. Da sua chaminé torta sai uma fina coluna de fumaça roxa pálida. Este é o Recanto da Alquimista.\nAo norte, um caminho de cascalho mais estreito se afasta da rua principal, ladeado por algumas casas simples. Uma placa de rua simples diz Travessa Oeste.\nA oeste, a rua termina no Portão Oeste, uma estrutura de madeira robusta que parece ser o limite da vila nessa direção.\nA leste, a rua principal leva de volta à área do Posto da Milícia.\nAo sul há o Recanto da Alquimista.\n",
			Saidas = new Dictionary<string, string>
			{
				["norte"] = "travessa_oeste_28",
				["sul"] = "recanto_alquimista",
				["leste"] = "rua_principal_14",
				["oeste"] = "portao_oeste_27"
			}
		},
		["recanto_alquimista"] = new RoomData
		{
			Nome = "(26) Recanto da Alquimista",
			Descricao = "O ar aqui dentro faz o nariz formigar, tendo uma mistura avassaladora de centenas de cheiros: enxofre, pétalas secas, poeira mineral e um toque metálico. O espaço é pequeno e abarrotado. A luz é fraca, vinda não de velas, mas do brilho suave de líquidos em frascos e de uma única lâmpada a óleo com uma chama verde pálida sobre o balcão. Prateleiras de madeira escura cobrem todas as paredes, do chão ao teto, repletas de incontáveis frascos de vidro, potes de cerâmica, caixas de madeira etiquetadas com uma caligrafia élfica elegante e pequenos sacos de pano. Estranhas partes de animais, como garras, penas e olhos em conserva, pendem das vigas do teto. Um balcão de madeira polida, mas marcado por queimaduras de ácido e manchas coloridas, separa a loja. Sobre ele há uma balança de latão delicada, um pilão de obsidiana e alguns frascos de poções prontos para venda. No fundo da loja, em uma área de trabalho, um complexo alambique de cobre borbulha suavemente, soltando a fina fumaça roxa que você viu lá fora. A dona, Gólirin, uma elfa de aparência acadêmica, está de costas, ajustando meticulosamente uma válvula no alambique. Ela não se vira, mas fala em voz clara: Limpe os pés. E não toque em nada, a menos que queira perder um dedo ou ganhar uma cauda. O que você precisa?\nA porta ao norte leva de volta à rua principal.\n",
			Saidas = new Dictionary<string, string> { ["norte"] = "rua_principal_15" }
		},
		["portao_oeste_27"] = new RoomData
		{
			Nome = "(27) Portão Oeste",
			Descricao = "Este é o limite oeste da vila. O Portão Oeste é uma barreira sólida de troncos grossos, reforçada com faixas de ferro. Tem sido deixado aberto, permitindo a passagem. Uma pesada tranca de madeira repousa em suportes ao lado, fora de uso. Um único guarda da milícia, Varten, está postado aqui, parecendo mais um vigia do que um soldado. Através do portão aberto, você pode ver a estrada a oeste. Ela leva a uma grande clareira cheia de tocos e serragem e, mais além, desaparece na escuridão iminente da Floresta Lethien.\nA estrada segue para o oeste, em direção ao perímetro dos lenhadores.\nA Rua Principal se estende para leste, de volta à vila.\n",
			Saidas = new Dictionary<string, string>
			{
				["oeste"] = "perimetro_lenhadores",
				["leste"] = "rua_principal_15"
			}
		},
		["travessa_oeste_28"] = new RoomData
		{
			Nome = "(28) Travessa Oeste",
			Descricao = "A rua é um caminho tranquilo e residencial de cascalho. O barulho da rua principal e das lojas parece distante. As casas são pequenas, feitas de madeira e pau-a-pique, com pequenos jardins frontais. Ao sul, o caminho de cascalho leva de volta à rua principal. A oeste, você vê a porta de uma pequena casa de madeira com uma janela escura.",
			Saidas = new Dictionary<string, string>
			{
				["norte"] = "travessa_oeste_29",
				["oeste"] = "npc_28",
				["sul"] = "rua_principal_15"
			}
		},
		["npc_28"] = new RoomData
		{
			Nome = "(28)Casa NPC",
			Descricao = "Apenas uma futura casa de npc. para voltar leste",
			Saidas = new Dictionary<string, string> { ["leste"] = "travessa_oeste_28" }
		},
		["travessa_oeste_29"] = new RoomData
		{
			Nome = "Travessa Oeste",
			Descricao = "A rua é um caminho tranquilo e residencial de cascalho. O barulho da rua principal e das lojas parece distante. A leste, você vê a porta de uma casa similar às outras, mas a madeira é tratada, a pintura da porta está fresca e pequenos vasos de ervas florescem na única janela, sugerindo um cuidado maior.\nA travessa continua para oeste, em direção a um casarão que se vê ao longe.\nA travessa continua para o sul, em direção a mais casas.\nA casa de Gólirin, a elfa, está ao leste.",
			Saidas = new Dictionary<string, string>
			{
				["norte"] = "npc_29",
				["sul"] = "travessa_oeste_28",
				["oeste"] = "travessa_oeste_30",
				["leste"] = "casa_golirin"
			}
		},
		["npc_29"] = new RoomData
		{
			Nome = "(29)Casa NPC",
			Descricao = "A futura casa de alguem. Para voltar ao sul",
			Saidas = new Dictionary<string, string> { ["sul"] = "travessa_oeste_29" }
		},
		["casa_golirin"] = new RoomData
		{
			Nome = "Casa Gólirin**",
			Descricao = "Casa da alquimista Gólirin. Para voltar digite oeste",
			Saidas = new Dictionary<string, string> { ["oeste"] = "travessa_oeste_29" }
		},
		["travessa_oeste_30"] = new RoomData
		{
			Nome = "(30) Travessa Oeste",
			Descricao = "O caminho de cascalho da Travessa Oeste termina aqui, abrindo-se para um pátio varrido de pedra em frente à estrutura residencial mais imponente da vila. O ar aqui é quieto, e a casa projeta uma longa sombra sobre a rua. Dois guardas da milícia, parecendo tensos e um pouco desconfortáveis em seu posto, flanqueiam o portão de ferro.\nA leste, a rua continua de volta à área residencial.\nA oeste, bloqueando o caminho, ergue-se o Casarão Bezeit.\n",
			Saidas = new Dictionary<string, string>
			{
				["leste"] = "travessa_oeste_29",
				["oeste"] = "casarao_bezeit"
			}
		},
		["casarao_bezeit"] = new RoomData
		{
			Nome = "Casarão Bezeit**",
			Descricao = "A maior casa da vila, onde mora a familia Bezeit, que remonta aos fundadores de Boeka. Volte ao leste",
			Saidas = new Dictionary<string, string> { ["leste"] = "travessa_oeste_30" }
		},
		["perimetro_lenhadores"] = new RoomData
		{
			Nome = "Perímetro dos Lenhadores",
			Descricao = "Diversos tocos de árvores e pedaços de troncos organizados em pilhas.",
			Saidas = new Dictionary<string, string>
			{
				["norte"] = "cabana_medel",
				["leste"] = "portao_oeste_27",
				["oeste"] = "floresta_lethien"
			}
		},
		["cabana_medel"] = new RoomData
		{
			Nome = "Cabana de Medel, o lenhador",
			Descricao = "Ele corte lenha. volte ao sul",
			Saidas = new Dictionary<string, string> { ["sul"] = "perimetro_lenhadores" }
		},
		["floresta_lethien"] = new RoomData
		{
			Nome = "Entrada da floresta Lethien",
			Descricao = "Entrada da floresta. Temos leste ou oeste",
			Saidas = new Dictionary<string, string>
			{
				["leste"] = "perimetro_lenhadores",
				["oeste"] = "floresta_lethien_1"
			}
		},
		["floresta_lethien_1"] = new RoomData
		{
			Nome = "Floresta Lethien 1",
			Descricao = "Mata densa com monstros. Temos norte, leste e oeste",
			Saidas = new Dictionary<string, string>
			{
				["norte"] = "floresta_lethien_2",
				["leste"] = "floresta_lethien",
				["oeste"] = "floresta_lethien_3"
			}
		},
		["floresta_lethien_2"] = new RoomData
		{
			Nome = "Floresta Lethien 2",
			Descricao = "Mata densa com monstros. Temos norte, sul e oeste",
			Saidas = new Dictionary<string, string>
			{
				["norte"] = "chefe_prisioneiro_medel",
				["sul"] = "floresta_lethien_1",
				["oeste"] = "floresta_lethien_4"
			}
		},
		["floresta_lethien_3"] = new RoomData
		{
			Nome = "Floresta Lethien 3",
			Descricao = "Mata densa com monstros. Temos norte, sul e leste",
			Saidas = new Dictionary<string, string>
			{
				["norte"] = "floresta_lethien_4",
				["sul"] = "caverna_lethien",
				["leste"] = "floresta_lethien_1"
			}
		},
		["floresta_lethien_4"] = new RoomData
		{
			Nome = "Floresta Lethien 4",
			Descricao = "Mata densa com monstros. Temos sul, oeste e leste",
			Saidas = new Dictionary<string, string>
			{
				["sul"] = "floresta_lethien_3",
				["leste"] = "floresta_lethien_2",
				["oeste"] = "clareira_lethien"
			}
		},
		["chefe_prisioneiro_medel"] = new RoomData
		{
			Nome = "Chefe missao Medel",
			Descricao = "Chefe com refem primo de medel. Temos sul",
			Saidas = new Dictionary<string, string> { ["sul"] = "floresta_lethien_2" }
		},
		["caverna_lethien"] = new RoomData
		{
			Nome = "Caverna Lethien",
			Descricao = "Pequena dungeon. O jogador encontra corpos de homens mutilados e marcas de batalha. De algum modo os homens lhe parecem familiares. No fim há um chefão.",
			Saidas = new Dictionary<string, string> { ["norte"] = "floresta_lethien_3" }
		},
		["clareira_lethien"] = new RoomData
		{
			Nome = "Clareira Lethien",
			Descricao = "Local onde jogador foi encontrado. Há alguns restos do acampamento. Temos leste e norte",
			Saidas = new Dictionary<string, string>
			{
				["norte"] = "entrada_templo_Lethe",
				["leste"] = "floresta_lethien_4"
			}
		},
		["entrada_templo_Lethe"] = new RoomData
		{
			Nome = "Entrada de Templo Abandonado",
			Descricao = "Circundado por diversas papoulas e ervas soníferas.",
			Saidas = new Dictionary<string, string>
			{
				["norte"] = "templo_lethe",
				["sul"] = "clareira_lethien"
			}
		},
		["templo_lethe"] = new RoomData
		{
			Nome = "Templo de Lethe",
			Descricao = "Uma dungeon que leva até uma grande estátua de uma mulher - a deusa do esquecimento, Lethe. Aqui o jogador descobre que, estando quase morto, fez um pacto no qual trocou suas memórias por sua vida. A deusa fala com ele e solicita algo.",
			Saidas = new Dictionary<string, string>
			{
				["oeste"] = "besta_lethe",
				["sul"] = "entrada_templo_Lethe"
			}
		},
		["besta_lethe"] = new RoomData
		{
			Nome = "Lar da Besta",
			Descricao = "Lar da Besta. Temos leste",
			Saidas = new Dictionary<string, string> { ["leste"] = "templo_lethe" }
		}
	};

	public override async void _Ready()
	{
		_textoPrincipal = GetNode<RichTextLabel>(_textoPrincipalPath);
		_entradaComando = GetNode<LineEdit>(_entradaComandoPath);
		
		if (_entradaComando != null)
		{
			_entradaComando.TextSubmitted += ProcessInput;
		}

		_javascriptBridgeAvailable = Engine.HasSingleton("JavaScriptBridge");

		await ToSignal(GetTree().CreateTimer(0.5f), Timer.SignalName.Timeout);

		CarregarSala(_salaIdAtual);
	}

	public void ProcessInput(string texto)
	{
		ExibirTexto("\n> " + texto);
		ProcessarComando(texto);
		_entradaComando.Clear();
	}

	private void ExibirTexto(string texto)
	{
		_textoPrincipal.AppendText(texto + "\n");
		_textoPrincipal.ScrollToLine(_textoPrincipal.GetLineCount() - 1);
	}

	private void CarregarSala(string novoId)
	{
		if (_salas.ContainsKey(novoId))
		{
			_salaIdAtual = novoId;
			var novaSala = _salas[novoId];
			
			string textoCompleto = $"--- {novaSala.Nome} ---\n{novaSala.Descricao}";
			ExibirTexto("\n" + textoCompleto);
			FalarTexto(novaSala.Nome + ". " + novaSala.Descricao);
		}
		else
		{
			string erro = $"Erro de mapa: Local {novoId} não existe.";
			ExibirTexto(erro);
			FalarTexto(erro);
		}
	}

	private void ProcessarComando(string comando)
	{
		string comandoLimpo = comando.ToLower().Trim();
		string[] palavras = comandoLimpo.Split(' ', StringSplitOptions.RemoveEmptyEntries);
		string resposta = "Comando não reconhecido.";
		
		if (!_salas.ContainsKey(_salaIdAtual))
		{
			resposta = "Erro interno: Sala atual não encontrada.";
			ExibirTexto(resposta);
			FalarTexto(resposta);
			return;
		}

		var salaData = _salas[_salaIdAtual];
		string direcao = string.Empty;

		if (palavras.Length == 1)
		{
			if (salaData.Saidas.ContainsKey(palavras[0]))
			{
				direcao = palavras[0];
			}
		}
		else if (palavras.Length >= 2)
		{
			string verbo = palavras[0];
			string alvo = palavras[1];
			
			if (verbo == "ir")
			{
				if (salaData.Saidas.ContainsKey(alvo))
				{
					direcao = alvo;
				}
				else
				{
					resposta = $"Você não pode ir para {alvo}.";
				}
			}
			else if (verbo == "olhar" && salaData.Saidas.ContainsKey(alvo))
			{
				resposta = $"Você olha para {alvo}. Está empoeirado.";
			}
			else
			{
				resposta = $"Comando '{verbo} {alvo}' não reconhecido.";
			}
		}
		
		if (!string.IsNullOrEmpty(direcao))
		{
			string idDestino = salaData.Saidas[direcao];
			
			if (_salas.ContainsKey(idDestino))
			{
				CarregarSala(idDestino);
				return;
			}
			else
			{
				resposta = $"O destino '{idDestino}' não é válido.";
			}
		}
		
		ExibirTexto(resposta);
		FalarTexto(resposta);
	}

	private void FalarTexto(string texto)
	{
		if (_javascriptBridgeAvailable)
		{
			string textoLimpo = EscapeJsString(texto);

			JavaScript.Eval($"falar_texto('{textoLimpo}')");
		}
	}
	
	public void IniciarComandoVoz()
	{
		if (_javascriptBridgeAvailable)
		{
			JavaScript.Eval("iniciar_escuta()");
		}
		else
		{
			FalarTexto("WebSpeech API não está disponível. Por favor, exporte para Web.");
		}
	}
	
	public void ReceberComandoVoz(string comando)
	{
		GD.Print($"Comando de voz recebido do JS: {comando}");
		
		if (!string.IsNullOrEmpty(comando))
		{
			ExibirTexto("\n> " + comando);
			ProcessarComando(comando);
		}
	}
	
	private string EscapeJsString(string text)
	{
		string result = text.Replace("\\", "\\\\");
		result = result.Replace("'", "\\'");
		result = result.Replace("\n", "\\n");
		result = result.Replace("\"", "\\\"");
		result = result.Replace("\t", " ");
		return result;
	}
}
