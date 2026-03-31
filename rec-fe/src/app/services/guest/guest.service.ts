import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../enviroment/enviroment';

export interface GuestRequest {
  firstName: string;
  secondName: string;
  lastName: string;
  idCard: string;
  phoneNumber: string;
  email: string;
}

export interface GuestResponse {
  id: number;
  firstName: string;
  secondName: string;
  lastName: string;
  idCard: string;
  phoneNumber: string;
  email: string;
}

@Injectable({
  providedIn: 'root'
})
export class GuestService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  createGuest(guest: GuestRequest): Observable<GuestResponse> {
    return this.http.post<GuestResponse>(`${this.apiUrl}/Guest/new`, guest);
  }

  getGuestsFromBooking(bookingId: number): Observable<GuestResponse[]> {
    return this.http.get<GuestResponse[]>(`${this.apiUrl}/Guest/from/${bookingId}`);
  }
}