// import { TestBed } from '@angular/core/testing';
// import { SignalRService } from './signalr.service';
// import { AuthService } from './auth.service';
// import * as signalR from '@microsoft/signalr';

// describe('SignalRService', () => {
//   let service: SignalRService;
//   let authServiceSpy: jasmine.SpyObj<AuthService>;

//   const mockHubConnection: any = {
//     start: jasmine.createSpy().and.returnValue(Promise.resolve()),
//     stop: jasmine.createSpy().and.returnValue(Promise.resolve()),
//     on: jasmine.createSpy(),
//     onclose: jasmine.createSpy(),
//     onreconnecting: jasmine.createSpy(),
//     onreconnected: jasmine.createSpy(),
//     invoke: jasmine.createSpy().and.returnValue(Promise.resolve())
//   };

//   // Store each step of the chain as a variable for clarity
//   const buildStep = { build: () => mockHubConnection };
//   const configureLoggingStep = { configureLogging: () => buildStep };
//   const reconnectStep = { withAutomaticReconnect: () => configureLoggingStep };
//   const urlStep = { withUrl: () => reconnectStep };

//   beforeAll(() => {
//     spyOn(signalR, 'HubConnectionBuilder').and.returnValue(urlStep as any);
//   });

//   beforeEach(() => {
//     authServiceSpy = jasmine.createSpyObj<AuthService>('AuthService', ['getAccessToken']);
//     authServiceSpy.getAccessToken.and.returnValue('mock-token');

//     TestBed.configureTestingModule({
//       providers: [
//         SignalRService,
//         { provide: AuthService, useValue: authServiceSpy }
//       ]
//     });

//     service = TestBed.inject(SignalRService);
//   });

//   it('should be created', () => {
//     expect(service).toBeTruthy();
//   });

//   it('should start connection and emit true', async () => {
//     await service.startConnection();
//     expect(mockHubConnection.start).toHaveBeenCalled();
//     service.connectionEstablished$.subscribe((value) => {
//       expect(value).toBeTrue();
//     });
//   });

//   it('should stop connection', async () => {
//     await service.stopConnection();
//     expect(mockHubConnection.stop).toHaveBeenCalled();
//   });

//   it('should join and leave inquiry groups', async () => {
//     await service.joinInquiryGroup('abc123');
//     expect(mockHubConnection.invoke).toHaveBeenCalledWith('JoinInquiryGroup', 'abc123');

//     await service.leaveInquiryGroup('abc123');
//     expect(mockHubConnection.invoke).toHaveBeenCalledWith('RemoveFromGroup', 'abc123');
//   });

//   it('should send a reply and handle errors', async () => {
//     await service.sendReply('abc123', 'Hello', 'Buyer');
//     expect(mockHubConnection.invoke).toHaveBeenCalledWith('SendReply', 'abc123', 'Hello', 'Buyer');

//     const consoleSpy = spyOn(console, 'error');
//     mockHubConnection.invoke.and.returnValue(Promise.reject('Mock error'));

//     try {
//       await service.sendReply('abc123', 'Hi again', 'Agent');
//     } catch (e) {
//       expect(consoleSpy).toHaveBeenCalledWith('Error sending reply:', 'Mock error');
//     }
//   });
// });
