<%@ Control Language="c#" Inherits="Ektron.Telerik.WebControls.EditorDialogControls.BaseDialogControl"%>

<table border="1" bordercolor="#000000" width="100%" cellpadding="2">
	<tr>
		<td colspan="3" align="middle"><strong>BOUTONS GENERAUX</strong></td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Img/ButtonDesign.gif"></td>
		<td>Bouton Mode Design - Passer eWebEdit400 en mode design.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Img/ButtonHtml.gif"></td>
		<td>Bouton Mode HTML - Passer eWebEdit400 en mode HTML.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Img/ButtonPreview.gif"></td>
		<td>Bouton Mode Prévisualisation - Passer eWebEdit400 en mode prévisualisation.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img src="<%= this.SkinPath %>Buttons/ConvertToUpper.gif"></td>
		<td>L'outil convertit le texte selectionné en majuscules, en préservant les éléments non-textuels, tels que les images, tableaux</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img src="<%= this.SkinPath %>Buttons/ConvertToLower.gif"></td>
		<td>L'outil convertit le texte sélectionné en minuscules, en préservant les éléments non-textuels, tels que les images, tableaux.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img src="<%= this.SkinPath %>Buttons/ImageMapDialog.gif"></td>
		<td>L'outil permet a l'utilisateur de créer des zones sensibles de différentes formes sur les images</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img src="<%= this.SkinPath %>Buttons/EktronImageModificationTool.gif"></td>
		<td>Modify you images with the Image Tool.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img src="<%= this.SkinPath %>Buttons/FormatCodeBlock.gif"></td>
		<td>Permet aux utilisateurs et de formater du code dans le contenu.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img src="<%= this.SkinPath %>Buttons/FontSize.gif"></td>
		<td>Menu déroulant qui permet à l'utilisateur d'appliquer une nouvelle taille de police en pixels, plutot qu'une taille fixe de 1 de 7 (comme le fait l'outil de taille de police).</td>
		<td>-</td>
	</tr>
	
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/ToggleScreenMode.gif"></td>
		<td>Bouton de changement du mode d'affichage - Passe eWebEdit400 en mode plein écran.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/ToggleTableBorder.gif"></td>
		<td>Bouton Montrer/Cacher - Montre ou cache les frontières autour des tableaux.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/ModuleManager.gif"></td>
		<td>Bouton Module Manager - active/arrête une liste de modules disponibles.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/ToggleDocking.gif"></td>
		<td>Bouton d'amarrage - accouple toutes les barres d'outils flottantes à leur secteur d'amarrage respectif.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/RepeatLastCommand.gif"></td>
		<td>Bouton Répéter - un raccourci pour répéter la dernière action exécutée.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/FindAndReplace.gif"></td>
		<td>Bouton Chercher/Remplacer - Cherche (et remplace) le texte dans l'éditeur.</td>
		<td>Ctrl+F</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/Print.gif"></td>
		<td>Bouton Impression - Imprime le contenu du eWebEdit400 ou de la page Web entière.</td>
		<td>Ctrl+P</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/Spellcheck.gif"></td>
		<td>Bouton Correcteur Orthographique - lance le correcteur orthographique.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/Cut.gif"></td>
		<td>Bouton Couper - coupe la sélection et l'insère dans le presse papier.</td>
		<td>Ctrl+X</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/Copy.gif"></td>
		<td>Bouton Copier - copie la sélection et l'insère dans le presse papier.</td>
		<td>Ctrl+C</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/Paste.gif"></td>
		<td>Bouton Coller - Colle le contenu copié dans le presse papier dans l'éditeur.</td>
		<td>Ctrl+Y</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/PastePlaintext.gif"></td>
		<td>Bouton Coller texte brut - Colle le texte (sans aucun formatage) dans l'éditeur.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/PasteFromWord.gif"></td>
		<td>Bouton Coller du Word - Colle du contenu  Word en nettoyant le code inutile.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/PasteAsHtml.gif"></td>
		<td>Bouton Coller du HTML - colle du code HTML dans l'éditeur et garde toutes les balises HTML.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/Undo.gif"></td>
		<td>Bouton Annuler - annuler les derniers changements.</td>
		<td>Ctrl+Z</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/Redo.gif"></td>
		<td>Bouton Refaire - Réapplique les derniers changements annulés.</td>
		<td>Ctrl+Y</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/Sweeper.gif"></td>
		<td>Bouton Retrait Formatage - Supprime les balises de formatage du texte.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/Help.gif"></td>
		<td>Bouton Aide Rapide - pour accèder à l'aide rapide (document actuel).</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/AboutDialog.gif"></td>
		<td>Bouton au Sujet de - Montre la version en cours et les qualifications de eWebEdit400.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" style="text-align: middle;"><strong>INSERER, MODIFIER ET GERER LES LIENS, 
			TABLEAUX, IMAGES MDIAS ET CARACTERES SPECIAUX </strong></td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/ImageManager.gif"></td>
		<td>Bouton gestionnaire d'Image - pour insérer une image à partir d'un répertoire prédéfini.</td>
		<td>Ctrl+G</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/ImageMapDialog.gif"></td>
		<td>Image map - Permet aux utilisateurs de définir des zones sensibles (avec liens) de formes différentes sur une image.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/EktronImageModificationTool.gif"></td>
		<td>Image Tool - Modify an image in the content.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/AbsolutePosition.gif"></td>
		<td>Bouton Position Absolue - Pour définir la position absolue d'un objet</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/InsertTable.gif"></td>
		<td>Bouton Tableau - Permet d'insérer un tableau dans l'éditeur.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/ToggleBorders.gif"></td>
		<td>Bouton Permuter les bordures - Permute les bordures de tous les tableaux de l'éditeur.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/InsertSnippet.gif"></td>
		<td>Bouton Extraits - Insère un extrait de texte ou de code HTML prédéfini.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/InsertFormElement.gif"></td>
		<td>Bouton Formulaire - Insère un élément de formulaire à partir d'un choix dans une liste déroulante.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/InsertDate.gif"></td>
		<td>Bouton Date - insère la date du jour.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/InsertTime.gif"></td>
		<td>Bouton Haure - insère l'heure courante.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/FlashManager.gif"></td>
		<td>Bouton Gestionnaire d'animation Flash - Insère une animation Flash et permet de définir ses propriétés.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/MediaManager.gif"></td>
		<td>Bouton Gestionnaire de Média - Insère un objet Windows Média (AVI, MPEG, WAV, etc.) et permet de définir ses propriétés.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/DocumentManager.gif"></td>
		<td>Bouton Gestionnaire de Document - Insère un document (PDF, Word,...) dans l'éditeur.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/LinkManager.gif"></td>
		<td>Bouton Hyperlien - Transforme le contenu sélectionné en hyperlien.</td>
		<td>Ctrl+K</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/Unlink.gif"></td>
		<td>Bouton Retire Hyperlien - Retire l'hyperlien du contenu sélectionné.</td>
		<td>Ctrl+Shift+K</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/Symbols.gif"></td>
		<td>Bouton Caractères Spéciaux. - Insère des caractères spéciaux.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/LinkManager.gif"></td>
		<td>Bouton Lien Prédéfini - Insère un lien interne ou externe partir d'une liste prédéfinie.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/TemplateManager.gif"></td>
		<td>Bouton Calibre - s'applique et calibre du HTML partir d'une liste prédéfinie de calibres.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" style="text-align: middle;"><strong style="font-weight: bold;">INSERER, MODIFIER ET
			FORMATER LES PARAGRAPHES </strong></td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/InsertParagraph.gif"></td>
		<td>Bouton Insère - Insère un nouveau paragraphe.</td>
		<td>Ctrl+M<br/>
			Ctrl+Enter</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/Paragraph.gif"></td>
		<td>Bouton Style - Applique le style standard au texte sélectionné.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/Outdent.gif"></td>
		<td>Bouton déplace Gauche - Déplace le paragraphe vers la gauche</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/Indent.gif"></td>
		<td>Bouton Déplace Droite - Déplace le paragraphe vers la droite</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/JustifyLeft.gif"></td>
		<td>Bouton Aligne Gauche - Aligne le paragraphe sélectionné à gauche.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/JustifyCenter.gif"></td>
		<td>Bouton Centre - Aligne le paragraphe sélectionné au centre.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/JustifyRight.gif"></td>
		<td>Bouton Aligne Droite - Aligne le paragraphe sélectionné à droite</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/JustifyFull.gif"></td>
		<td>Bouton Justifie - Justifie le paragraphe choisi.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/InsertUnorderedList.gif"></td>
		<td>Bouton Puces - Insère des puces devant la sélection.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/InsertOrderedList.gif"></td>
		<td>Bouton Numérotation - Insère une numérotation devant la sélection.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/InsertHorizontalRule.gif"></td>
		<td>Bouton Ligne - Insère une ligne horizontale à l'emplacement du curseur.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" style="text-align: middle;"><strong>INSERER, MODIFIER ET 
			FORMATER LE TEXTE ET LES LISTES </strong></td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/Bold.gif"></td>
		<td>Bouton Gras - Applique un format gras à la sélection.</td>
		<td>Ctrl+B</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/Italic.gif"></td>
		<td>Bouton Italique - Applique un format italique à la sélection.</td>
		<td>Ctrl+I</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/Underline.gif"></td>
		<td>Bouton Soulign - Applique un format souligné à la sélection.</td>
		<td>Ctrl+U</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/Superscript.gif"></td>
		<td>Bouton Exposant - Applique un format exposant à la sélection.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/Subscript.gif"></td>
		<td>Bouton Indice - Applique un format indice à la sélection.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/FontName.gif"></td>
		<td>Bouton Police - Sélection de la police de caractères.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/FontSize.gif"></td>
		<td>Bouton Taille - Sélection de la taille de la police.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/ForeColor.gif"></td>
		<td>Bouton Coleur - Sélection de la couleur du texte.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/BackColor.gif"></td>
		<td>Bouton Fond - Changer de la couleur de l'arrière plan du texte.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/Class.gif"></td>
		<td>Bouton Style - Applique un style prédfini au texte sélectionné.</td>
		<td>-</td>
	</tr>
	<tr>
		<td style="text-align: middle;"><img src="<%= this.SkinPath %>Buttons/Class.gif"></td>
		<td>Enlève Formatage - Enlève le formatage du texte choisi ou entier.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" style="text-align: middle;"><strong>AUTRES RACCOURCIS CLAVIER</strong></td>
	</tr>
	<tr>
		<td>-</td>
		<td>Sélectionnet tous les textes, images et tables dans l'éditeur.</td>
		<td>Ctrl+A</td>
	</tr>
	<tr>
		<td>-</td>
		<td>Recherche dans la page </td>
		<td>Ctrl+F</td>
	</tr>
	<tr>
		<td>-</td>
		<td>Fermer la fentre active. </td>
		<td>Ctrl+W</td>
	</tr>
	<tr>
		<td>-</td>
		<td>Fermer l'application active.</td>
		<td>Ctrl+F4</td>
	</tr>
</table>