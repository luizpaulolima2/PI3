// $(document).ready(function(){


// 	//chama função resposavel por deixar menu de categorias responsivo.
// 	responsive_menu();
	
// 	//chama função resposavel por deixar menu de categorias responsivo, quando a tela mudar de tamanho.
// 	$(window).resize(function(){
// 		responsive_menu();
// 	});

// 	//fecha e abre menu de categorias em telas mobiles.
// 	$('.btnCategoria').click(function(e){ 
// 		$(".menu-categoria").toggle();
// 	});
	
// 	$('.menu-categoria li').hover(
//         function(e){ 
//     $(this).animate({
//       	width:'+=3px',
//     });
//     },
//     function(e){
//         $(this).animate({
//     width:'-=3px',
//        });
//     });

// 	// Efeito sobre produto
// 	$('.product-hover').hover(
// 		function(e){
// 			$(this).animate({opacity:0.94});
// 		},
// 		function(e){
// 			$(this).animate({opacity:0});
// 	});

	
// 	//função responsavel por deixar menu categorias responsivo.
// 	function responsive_menu(){
// 		var screen_size = $(document).width();
// 		if(screen_size <= 768 ){
// 			$( ".menu-categoria" ).css('display','none');
// 		}else {
// 			$( ".menu-categoria" ).css('display','block');
// 		}
// 	}
// // 

// });
