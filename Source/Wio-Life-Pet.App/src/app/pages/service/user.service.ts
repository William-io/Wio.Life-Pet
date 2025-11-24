import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { URL_API } from '../../settings/api/config';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  constructor(private http: HttpClient) { }

  getUsers(req: any) {
    return this.http.post(`${URL_API}/api/user/list`, req).toPromise()
    .then(response => response);
  }
}
