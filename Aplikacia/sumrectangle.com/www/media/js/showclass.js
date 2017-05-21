var ulohy = {};
var class_id;
var class_name;
/*function showExams() {
    ulohy = {};
    var resultRef = firebase.database().ref('exams/'+getCookie("teacherID")+"/");
    resultRef.on('child_added', function(data) {
        $("<li><label for="+data.key+"><input type=checkbox name='"+data.val().examsName+"' value='"+data.val().examsName+"' id="+data.key+">"+data.val().examsName+"</label></li>").appendTo("#pridajUlohu .priklady");
        $("input[name="+data.val().examsName+"]").change(function(){
            if($(this).is(':checked')) {
                ulohy[this.id.toString()] = this.value;
            } else {
                delete ulohy[this.id.toString()];
            }
        });
    });
}*/

function ulozUlohy() {
    var resultRef = firebase.database().ref(class_id+'/exams').update(ulohy);
    $('.priklady').empty();
    flashMsg("success","Ulohy boli ulozene");
}

function deleteExam($key) {
    if( PopupAlert("cvicenie ?") ) {
        var ref = firebase.database().ref(class_id+'/exams/'+$key.data.param1);
        ref.remove(function(error) {
            $("#zobrazUlohy #"+$key.data.param1).remove();
            console.log('remove')
        });
        flashMsg("success","Uloha bola odstranena");
    }
}

$( document ).ready(function() {
    $('.pridaj_ulohu').on("click",function(){
        $("#pridajUlohu .priklady").empty();
        ulohy = {};
        var resultRef = firebase.database().ref('exams/'+getCookie("teacherID")+"/");
        resultRef.on('child_added', function(data) {
            $("<li><label for="+data.key+"><input type=checkbox name='"+data.val().examsName+"' value='"+data.val().examsName+"' id="+data.key+">"+data.val().examsName+"</label></li>").appendTo("#pridajUlohu .priklady");
            $("input[name="+data.val().examsName+"]").change(function(){
                if($(this).is(':checked')) {
                    ulohy[this.id.toString()] = this.value;
                } else {
                    delete ulohy[this.id.toString()];
                }
            });
        });
        class_id =  $(this).attr("id");
        class_name =  $(this).attr("name");
        $("#pridajU").text("Pridaj ulohy: "+class_name);
    })

    $('.zobraz_ulohy').on("click",function(){
        $("#zobrazUlohy .priklady").empty();
        class_id =  $(this).attr("id");
        class_name =  $(this).attr("name");
        var resultRef = firebase.database().ref(class_id+'/exams');
        resultRef.on('child_added', function(data) {
            $("<li id="+data.key+"><a href='showexam?pid="+data.key+"&name="+data.val()+"' class='open "+data.key+"'>" + data.val() + "</a><a class='close' ><i class='fa fa-window-close-o' aria-hidden=true></i></a></li>").appendTo("#zobrazUlohy .priklady");
            $("a.close").click({param1: data.key}, deleteExam);
        });
    })



});