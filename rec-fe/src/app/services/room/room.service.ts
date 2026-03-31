import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from './../../../enviroment/enviroment';

export interface Room {
  id: number;
  roomNumber: string;
  roomType: string;
  price: number;
  capacity: number;
  occupied: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class RoomService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getAllRooms(): Observable<Room[]> {
    return this.http.get<Room[]>(`${this.apiUrl}/Room/All`);
  }

  getRoomByNumber(roomNumber: string): Observable<Room> {
    return this.http.get<Room>(`${this.apiUrl}/Room/${roomNumber}`);
  }

  getRoomsByType(roomType: string): Observable<Room[]> {
    return this.http.get<Room[]>(`${this.apiUrl}/Room/All/Type/${roomType}`);
  }

  updateOccupation(roomId: number, occupied: boolean): Observable<any> {
    return this.http.put(`${this.apiUrl}/Room/occupation/id/${roomId}/${occupied}`, {});
  }
}