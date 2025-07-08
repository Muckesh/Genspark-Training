import { HttpTestingController, provideHttpClientTesting } from "@angular/common/http/testing";
import { AgentService } from "./agent.service";
import { environment } from "../../environments/environment";
import { TestBed } from "@angular/core/testing";
import { provideHttpClient } from "@angular/common/http";
import { Agent, UpdateAgentDto } from "../models/agent.model";
import { RegisterAgentDto } from "../models/auth-dto.model";

describe('Agent Service',()=>{
    let service:AgentService;
    let httpMock: HttpTestingController;
    const baseUrl= `${environment.apiUrl}/agents`;

    const mockAgent: Agent = {
    id: '1',
    agencyName: 'ABC Realty',
    licenseNumber: 'LIC12345',
    phone: '9876543210',
    user: {
      id: 'u1',
      name: 'Agent Smith',
      email: 'smith@example.com',
      role: 'Agent',
    //   createdAt: Date.now(),
      isDeleted:false
    }
  };

  const registerPayload: RegisterAgentDto = {
    name: 'Agent Smith',
    email: 'smith@example.com',
    password: 'pass123',
    agencyName: 'ABC Realty',
    phone: '9876543210',
    licenseNumber: 'LIC12345'
  };

  const updatePayload: UpdateAgentDto = {
    agencyName: 'Updated Realty',
    licenseNumber: 'NEW123',
    phone: '9999999999'
  };

    beforeEach(()=>{
        TestBed.configureTestingModule({
            providers:[
                AgentService,
                provideHttpClient(),
                provideHttpClientTesting()
            ]
        });
        service=TestBed.inject(AgentService);
        httpMock=TestBed.inject(HttpTestingController);
    });

    afterEach(()=>{
        httpMock.verify();
    });
    it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch all agents with filtered params', () => {
    const rawResponse = {
      pageNumber: 1,
      pageSize: 10,
      totalCount: 1,
      items: {
        $values: [mockAgent]
      }
    };

    const queryParams = {
      pageNumber: 1,
      pageSize: 10,
      agencyName: '',
      licenseNumber: null
    };

    service.getAllAgents(queryParams).subscribe(result => {
      expect(result.pageNumber).toBe(1);
      expect(result.items.length).toBe(1);
      expect(result.items[0].agencyName).toBe('ABC Realty');
    });

    const req = httpMock.expectOne((r) =>
      r.url === baseUrl &&
      r.params.get('pageNumber') === '1' &&
      r.params.get('pageSize') === '10'
    );

    expect(req.request.method).toBe('GET');
    req.flush(rawResponse);
  });

  it('should register a new agent', () => {
    service.registerAgent(registerPayload).subscribe(res => {
      expect(res.message).toBe('Agent registered successfully');
    });

    const req = httpMock.expectOne(`${baseUrl}/register`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(registerPayload);

    req.flush({ message: 'Agent registered successfully' });
  });

  it('should update an existing agent', () => {
    const updatedAgent: Agent = {
      ...mockAgent,
      ...updatePayload
    };

    service.updateAgent('1', updatePayload).subscribe(res => {
      expect(res.agencyName).toBe('Updated Realty');
    });

    const req = httpMock.expectOne(`${baseUrl}/1`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(updatePayload);

    req.flush(updatedAgent);
  });

  it('should transform raw agent response correctly', () => {
    const raw = {
      id: '42',
      agencyName: 'Prime Estates',
      licenseNumber: 'LIC98765',
      phone: '1112223333',
      user: {
        id: 'u42',
        name: 'Neo',
        email: 'neo@example.com'
      }
    };

    const transformed = (service as any).transformAgent(raw);
    expect(transformed.agencyName).toBe('Prime Estates');
    expect(transformed.user.name).toBe('Neo');
  });

});