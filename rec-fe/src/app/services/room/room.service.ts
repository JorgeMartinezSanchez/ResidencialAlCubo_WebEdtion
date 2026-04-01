import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, Observable } from 'rxjs';
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
    console.log('Fetching rooms from:', `${this.apiUrl}/Room/All`);
    return this.http.get<Room[]>(`${this.apiUrl}/Room/All`).pipe(
      catchError(error => {
        console.error('Error fetching rooms:', error);
        console.error('Error details:', {
          status: error.status,
          statusText: error.statusText,
          message: error.message,
          url: error.url
        });
        throw error;
      })
    );
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