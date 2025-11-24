import { Component, OnInit } from '@angular/core';
import { UserService } from '../service/user.service';

@Component({
  selector: 'app-user',
  imports: [],
  template: `
  
  <div class="card">
    <div class="font-semibold text-xl mb-4">Usuários</div>
  </div>
  
  
  
  
  `,
  styleUrl: './user.scss'
})


export class User implements OnInit{

  req = {
    index: 0,
    limit: 10
  };

  constructor(public userService: UserService) { }
  
  ngOnInit(): void {
    this.List(this.req);
  }

  List(request: any){
    try {
      this.userService.getUsers(request).then((response:any)=>{
        console.log(response);
      });
    } catch (error) {
      console.error('Erro ao listar usuários:', error);
    }
  }
}


