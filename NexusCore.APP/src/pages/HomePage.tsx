import { Container } from "../components/pages/Container";

export type HomePageProps = {
    backgroundColor: string;
};

export const HomePage = ({ 
    backgroundColor
}: HomePageProps) => {
    return (
        <Container bgColor={backgroundColor}>
            <div className={`text-3xl text-black`}>HomePage</div>
        </Container>
    );
};