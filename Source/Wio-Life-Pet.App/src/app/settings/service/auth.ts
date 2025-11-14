import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { URL_API } from '../api/config';
import { sha512 } from 'js-sha512';

@Injectable({
  providedIn: 'root'
})
export class Auth {

  constructor(private http: HttpClient) { }

  isLoggedIn(): boolean {
    let token = localStorage.getItem('token_sigvet');
    if(token == null){
      return false;
    }

    return true;
  }

  login(data: {username: string, passwordHash: string}) {
    console.log(URL_API); 
    return this.http.post(`${URL_API}/login`, {
      username: data.username,
      passwordHash: sha512(data.passwordHash).toString().toUpperCase()
    }).toPromise().then((res) => res);
  }
  
}